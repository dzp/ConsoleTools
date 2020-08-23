using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClassLibraryGenFileInfo;

namespace GenSturctByDBC
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("no file");
                Console.ReadKey();
                return;
            }
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            //currentPath += "\\RTE";
            System.IO.Directory.CreateDirectory(currentPath);
            currentPath += "\\";
            Console.WriteLine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            foreach (var filePath in args)
            {
                Console.WriteLine("current file is {0:S}", Path.GetFileName(filePath));
                Regex searchValTabRegex = new Regex(@"^VAL_TABLE_\s(\w+)\s((\d+\s\x22\w+\x22\s)+)\;");
                Regex searchMsgRegex = new Regex(@"^BO_\s(\d+)\s(\w+):\s(\d)\s[\w+\d*]+");
                Regex searchSigRegex = new Regex(@"^\sSG_\s([\w+\d*]+)\s:\s(\d+)\|(\d+)\@([0,1])([+-])\s\(([-+0-9.]+)\,([-+0-9.]+)\)\s\[([-+0-9.]+)\|([-+0-9.]+)\]\s\x22([\s+\S+\d+\D+\w+\W+]+)\x22\s([\w+\d*]+)$");

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                FileStream newfshFile = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +"drv_"+ Path.GetFileNameWithoutExtension(filePath) + "_CanMsgType.h", FileMode.Create);
                FileStream newfscFile = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +"drv_"+ Path.GetFileNameWithoutExtension(filePath) + "_CanMsgTypeIF.c", FileMode.Create);
                StreamWriter streamWriterHFile = new StreamWriter(newfshFile);
                StreamWriter streamWriterCFile = new StreamWriter(newfscFile);
                ClassWriteFile writeHFile = new ClassWriteFile(streamWriterHFile,
                    "drv_"+Path.GetFileNameWithoutExtension(filePath)+ "_CanMsgType.h", "Daizipeng");
                ClassWriteFile writeCFile = new ClassWriteFile(streamWriterCFile,
                    "drv_"+Path.GetFileNameWithoutExtension(filePath) + "_CanMsgType.c", "Daizipeng");
                writeHFile.WriteFileHead(null);
                writeHFile.WriteFileMacro(null);
                StreamReader streamReader = new StreamReader(fs);
                string str;
                int lineNum = 0;
                string currentName;
                string currentId;
                string currentDlc;
                List<ClassDBCSig> currentClassDbcSigs = new List<ClassDBCSig>();
                List<ClassDBCMsg> currentClassDbcMsgs = new List<ClassDBCMsg>();
                
                while ((str = streamReader.ReadLine()) != null)
                {
                    lineNum++;
                    Match m = searchValTabRegex.Match(str);
                    if (m.Success)
                    {
                        Console.WriteLine(@"search a signal Val Tab  in {0:d} line : Val Tab is {1:s}", lineNum,
                            m.Groups[2]);
                        writeHFile.WriteEnum(m.Groups[2].ToString().Replace("\"", ""), "Sig_" + m.Groups[1].ToString());
                        
                    }

                    Match msgMatch = searchMsgRegex.Match(str);
                    if (msgMatch.Success)
                    {
                        Console.WriteLine("msgId:0x{0:x8},msgName:{1:s},dlc:{2:d}",
                            Convert.ToUInt32(msgMatch.Groups[1].ToString()),
                            msgMatch.Groups[2], msgMatch.Groups[3]);
                        currentId = String.Format("0x{0:x8}",Convert.ToUInt32(Convert.ToUInt32(msgMatch.Groups[1].ToString())));
                        ClassDBCMsg currentClassDbcMsg = new ClassDBCMsg(msgMatch.Groups[2].ToString(),Convert.ToUInt32(msgMatch.Groups[1].ToString()) & 0x7fffffff,
                            Convert.ToUInt32(msgMatch.Groups[1].ToString())>=0x80000000?E_FrameFormat.e_FrameFormat_EXT:E_FrameFormat.e_FrameFormat_STD,
                            E_FrameType.e_FrameType_DATA);
                        writeHFile.WriteStruct(currentClassDbcSigs, currentClassDbcMsg);
                        currentClassDbcSigs.Clear();
                        currentClassDbcMsgs.Clear();
                        
                    }

                    Match sigMatch = searchSigRegex.Match((str));
                    if (sigMatch.Success)
                    {
                        Console.WriteLine("sigName:{0:s} startBit:{1:s} len:{2:s} byteOrder:{3:s} valueType:{4:s} Factor:{5:s} " +
                                          "offset:{6:s} min:{7:s} max:{8:s} unit:{9:s} receiver:{10:s}",sigMatch.Groups[1],
                            sigMatch.Groups[2], sigMatch.Groups[3], sigMatch.Groups[4], sigMatch.Groups[5], sigMatch.Groups[6],
                            sigMatch.Groups[7], sigMatch.Groups[8], sigMatch.Groups[9], sigMatch.Groups[10], sigMatch.Groups[11]);
                        ClassDBCSig sigInfoClassDbcSig = new ClassDBCSig(sigMatch.Groups[1].ToString(),
                            Convert.ToUInt32(sigMatch.Groups[2].ToString()),
                            Convert.ToUInt32(sigMatch.Groups[3].ToString()),
                            (E_DBCByteOrder)Convert.ToUInt32(sigMatch.Groups[4].ToString()));
                        

                        if (sigInfoClassDbcSig.byteOrder == E_DBCByteOrder.E_ByteOrderMotor)
                        {
                            sigInfoClassDbcSig.startBit = sigInfoClassDbcSig.startBit + 1 - sigInfoClassDbcSig.len;
                        }
                        currentClassDbcSigs.Add(sigInfoClassDbcSig);

                    }
                }

                writeHFile.WriteFileEnd();
                Console.ReadKey();

            }
        }
    }
}
