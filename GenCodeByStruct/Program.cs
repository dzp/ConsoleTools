using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenCodeByStruct
{
    class Program
    {
        private static void InsFunctionDeclaration(StreamWriter streamWriter, string structName,string memberName, string memberType,string memberAryLen = "0")
        {
            if(memberAryLen == "0")
            {
                streamWriter.WriteLine("extern ret_Results Rte_Get_" + structName + memberName + "(" + memberType + "* p_val);");
                streamWriter.WriteLine("extern ret_Results Rte_Set_" + structName + memberName + "(" + memberType + " val);");
            }
            else
            {
                streamWriter.WriteLine("extern ret_Results Rte_Get_" + structName + memberName + "(" + memberType + "* p_val);");
                streamWriter.WriteLine("extern ret_Results Rte_Set_" + structName + memberName + "(" + memberType + "* p_val);");
            }
            
        }
        private static void InsFunctionEntity(StreamWriter streamWriter, string structName, string memberName, string memberType, string memberAryLen = "0")
        {
            if (memberAryLen == "0")
            {
                streamWriter.WriteLine("extern ret_Results Rte_Get_" + structName + memberName + "(" + memberType + "* p_val)");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tret_Results ret = E_OK;");
                streamWriter.WriteLine("\tdis_irq();");
                streamWriter.WriteLine("\t*p_val = " + structName + "." + memberName + ";");
                streamWriter.WriteLine("\ten_irq();");
                streamWriter.WriteLine("\treturn ret;");
                streamWriter.WriteLine("}");

                streamWriter.WriteLine();
                streamWriter.WriteLine("extern ret_Results Rte_Set_" + structName + memberName + "(" + memberType + " val)");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tret_Results ret = E_OK;");
                streamWriter.WriteLine("\tdis_irq();");
                streamWriter.WriteLine("\t" + structName + "." + memberName + " = val;");
                streamWriter.WriteLine("\ten_irq();");
                streamWriter.WriteLine("\treturn ret;");
                streamWriter.WriteLine("}");
            }
            else
            {
                streamWriter.WriteLine("extern ret_Results Rte_Get_" + structName + memberName + "(" + memberType + "* p_val)");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tret_Results ret = E_OK;");
                streamWriter.WriteLine("\tdis_irq();");
                //streamWriter.WriteLine("\t*p_val = " + structName + "." + memberName + ";");
                streamWriter.WriteLine("\tmemcpy((void*)p_val,(void*)" + structName + "." + memberName + "," + "sizeof("+ structName + "." + memberName+")" +");");
                streamWriter.WriteLine("\ten_irq();");
                streamWriter.WriteLine("\treturn ret;");
                streamWriter.WriteLine("}");

                streamWriter.WriteLine();
                streamWriter.WriteLine("extern ret_Results Rte_Set_" + structName + memberName + "(" + memberType + "* p_val)");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tret_Results ret = E_OK;");
                streamWriter.WriteLine("\tdis_irq();");
                //streamWriter.WriteLine("\t" + structName + "." + memberName + " = val;");
                streamWriter.WriteLine("\tmemcpy((void*)" + structName + "." + memberName + ",(void*)p_val," + "sizeof(" + structName + "." + memberName + ")" + ");");
                streamWriter.WriteLine("\ten_irq();");
                streamWriter.WriteLine("\treturn ret;");
                streamWriter.WriteLine("}");
            }

        }
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
            foreach (string filePath in args)
            {
                Console.WriteLine("current file is {0:S}", Path.GetFileName(filePath));
                Regex regex_searchStructHead = new Regex(@"^\s*typedef\s*struct\s*(\w*)\s*{");
                //Regex regex_searchMemberVal = new Regex(@"^\s*\w+\d*\s+(\w+\s*)+:*[\d*]*[^{}];");
                //Regex regex_searchMemberVal = new Regex(@"^\s*(\w+\d*)\s+(\w*\s+)*(\w+):*\d*[^{}]*;\s*$");
                Regex regex_searchMemberVal = new Regex(@"^\s*(\w+\d*)\s+(\w*\s+)*(\w+)(\[?(\d*)\]?):*\d*[^{}]*;\s*$");
                Regex regex_searchStructTypeName = new Regex(@"^\s*}\s*(\w+)\s*;\s*$");

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                FileStream newfshFile = new FileStream(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.GetFileNameWithoutExtension(filePath)+"IF.h", FileMode.Create);
                FileStream newfscFile = new FileStream(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.GetFileNameWithoutExtension(filePath) + "IF.c", FileMode.Create);
                StreamWriter streamWriterHFile = new StreamWriter(newfshFile);
                StreamWriter streamWriterCFile = new StreamWriter(newfscFile);
                
                streamWriterHFile.WriteLine(@"#ifndef __" + Path.GetFileNameWithoutExtension(filePath) + "_H__");
                streamWriterHFile.WriteLine(@"#define __" + Path.GetFileNameWithoutExtension(filePath) + "_H__");
                streamWriterHFile.WriteLine(@"#include "+"\""+ Path.GetFileName(filePath)+"\"");
                streamWriterHFile.Flush();
                streamWriterCFile.WriteLine(@"#include " + "\"" + Path.GetFileNameWithoutExtension(filePath) + "IF.h" + "\"");
                streamWriterCFile.WriteLine(@"#include " + "\"string.h\"");
                streamWriterCFile.Flush();
                StreamReader streamReader = new StreamReader(fs);
                string str;
                int lineNum = 0;
                List<string> memberName = new List<string>();
                List<string> memberType = new List<string>();
                List<string> memberAryLen = new List<string>();
                bool hasAStrut = false;
                while((str = streamReader.ReadLine())!= null)
                {
                    lineNum++;
                    Match m = regex_searchStructHead.Match(str);
                    if (m.Success)
                    {
                        Console.WriteLine("search a struct tag in {0:d} line : struct Tag is {1:s}", lineNum,m.Groups[1]);
                        memberName.Clear();
                        memberType.Clear();
                        memberAryLen.Clear();
                        hasAStrut = true;
                    }
                    else
                    {
                    }
                    Match mv = regex_searchMemberVal.Match(str);
                    if (mv.Success)
                    {
                        Console.WriteLine("search a struct member in {0:d} line : member type is {1:s}, member name is  {2:s}", lineNum, mv.Groups[1], mv.Groups[3]);
                        memberType.Add(mv.Groups[1].ToString());
                        memberName.Add(mv.Groups[3].ToString());
                        if (mv.Groups[5].ToString() != "")
                        {
                            memberAryLen.Add(mv.Groups[5].ToString());
                        }
                        else
                        {
                            memberAryLen.Add("0");
                        }
                        
                    }
                    else
                    {
                    }
                    Match mtn = regex_searchStructTypeName.Match(str);
                    if ((mtn.Success)&&(hasAStrut))
                    {
                        Console.WriteLine("search a struct type in   {0:d} line : struct type name is {1:s}", lineNum, mtn.Groups[1]);
                        streamWriterCFile.WriteLine("volatile " + mtn.Groups[1] + "\t" + mtn.Groups[1] + "IF;");
                        for(int ind = 0; ind < memberName.Count; ind++)
                        {
                            InsFunctionDeclaration(streamWriterHFile,mtn.Groups[1].ToString()+"IF", memberName[ind], memberType[ind],memberAryLen[ind]);
                            InsFunctionEntity(streamWriterCFile, mtn.Groups[1].ToString()+"IF", memberName[ind], memberType[ind], memberAryLen[ind]);
                            streamWriterCFile.WriteLine();
                        }
                        streamWriterHFile.WriteLine();
                        streamWriterHFile.WriteLine();
                        hasAStrut = false;
                        //streamWriterCFile.Flush();
                    }
                    else
                    {
                    }
                }
                streamWriterCFile.Flush();
                streamWriterCFile.Close();
                streamWriterHFile.WriteLine(@"#endif /* " + Path.GetFileNameWithoutExtension(filePath) + "_H__ */");
                streamWriterHFile.Flush();
                streamWriterHFile.Close();


            }
            
            Console.ReadKey();
        }
    }
}
