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
                Regex searchMsgRegex = new Regex(@"^BO_\s(\d+)\s(\w+):\s(\d)\s\w+");

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                FileStream newfshFile = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +"drv_"+ Path.GetFileNameWithoutExtension(filePath) + "_CanMsgType.h", FileMode.Create);
                FileStream newfscFile = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +"drv_"+ Path.GetFileNameWithoutExtension(filePath) + "_CanMsgTypeIF.c", FileMode.Create);
                StreamWriter streamWriterHFile = new StreamWriter(newfshFile);
                StreamWriter streamWriterCFile = new StreamWriter(newfscFile);
                ClassWriteFile writeHFile = new ClassWriteFile(streamWriterHFile, "drv_"+Path.GetFileNameWithoutExtension(filePath)+ "_CanMsgType.h", "Daizipeng");
                ClassWriteFile writeCFile = new ClassWriteFile(streamWriterCFile, "drv_"+Path.GetFileNameWithoutExtension(filePath) + "_CanMsgType.c", "Daizipeng");
                writeHFile.WriteFileHead(null);
                writeHFile.WriteFileMacro(null);
                StreamReader streamReader = new StreamReader(fs);
                string str;
                int lineNum = 0;
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
                        Console.WriteLine("msgId:0x{0:x8},msgName:{1:s},dlc:{2:d}", Convert.ToUInt32(msgMatch.Groups[1].ToString()), msgMatch.Groups[2], msgMatch.Groups[3]);




                    }
                }

                writeHFile.WriteFileEnd();
                //Console.ReadKey();

            }
        }
    }
}
