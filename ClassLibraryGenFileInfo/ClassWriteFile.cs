using System;
using System.IO;

namespace ClassLibraryGenFileInfo
{
    public class ClassWriteFile
    {
        private StreamWriter CurrentFileStream;
        private string Author;
        private string Date;
        private string FileHead;
        private string CommentsBarStart = "/*******************************************************************************";
        private string CommentsBarEnd = "******************************************************************************/";
        private string FileName;
        public ClassWriteFile(StreamWriter writer,string fileName, string author)
        {
            CurrentFileStream = writer;
            Author = author;
            FileName = fileName;
            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (writer == null)
            {
                throw new Exception("file stream is null");
            }

        }

        public int WriteFileEnd(StreamWriter writer = null)
        {
            if (writer == null)
            {
                writer = CurrentFileStream;
            }
            string macroFileName = FileName.Replace(".", "_");
            writer.WriteLine(@"#endif /* __" + macroFileName.ToUpper() + @"__ */");
            writer.Flush();
            return 0;
        }

        public int WriteEnum(string v_k, string enumName, StreamWriter writer = null)
        {
            if (writer == null)
            {
                writer = CurrentFileStream;
            }

            writer.WriteLine();
            enumName = "E_" + enumName;
            string[] v_kArry = v_k.Split(' ');
            writer.WriteLine("typedef enum " + enumName + "Tag{");
            for (int i = 0; i < (v_kArry.Length - 1); i+=2)
            {
                writer.WriteLine("\t" + enumName + "_" + v_kArry[i + 1] + " = " + v_kArry[i]+",");
            }
            writer.WriteLine("}" + enumName + ";");
            return 0;
        }
        public int WriteFileMacro(string Macro, StreamWriter writer = null)
        {
            if (writer == null)
            {
                writer = CurrentFileStream;
            }

            string macroFileName = FileName.Replace(".", "_");
            writer.WriteLine(@"#ifndef __" + macroFileName.ToUpper() + "__");
            writer.WriteLine(@"#define __" + macroFileName.ToUpper() + "__");
            writer.Flush();
            return 0;
        }

        public int WriteFileHead(string commit, StreamWriter writer = null)
        {
            if (writer == null)
            {
                writer = CurrentFileStream;
            }

            writer.WriteLine(CommentsBarStart);
            writer.WriteLine(" * @file " + FileName);
            writer.WriteLine(" * @brief GenCode"+commit);
            writer.WriteLine(" * @author " + Author);
            writer.WriteLine(" * @version 0.8");
            writer.WriteLine(" * @date " + Date);
            writer.WriteLine(CommentsBarEnd);

            writer.Flush();
            return 0;
        }
    }
}
