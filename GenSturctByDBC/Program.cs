using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                Regex searchValTabRegex = new Regex(@"^VAL_TABLE_\s\w+\s((\d+\s\x22\w+\x22\s)+)\;");

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                FileStream newfshFile = new FileStream("dvr_" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.GetFileNameWithoutExtension(filePath) + "_CanMsgType.h", FileMode.Create);
                FileStream newfscFile = new FileStream("dvr_" + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.GetFileNameWithoutExtension(filePath) + "_CanMsgTypeIF.c", FileMode.Create);
                StreamWriter streamWriterHFile = new StreamWriter(newfshFile);
                StreamWriter streamWriterCFile = new StreamWriter(newfscFile);
            }
        }
    }
}
