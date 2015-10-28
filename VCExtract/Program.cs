using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            var csvFilePath = args[0]; 
            var output = args[1];
            var paths = Parse(csvFilePath)
                .Where(e => e.Result == "SUCCESS")
                .Select(e => e.Path)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var path in paths)
            {
                try
                {
                    var drive = Path.GetPathRoot(path);
                    var basePath = path.Substring(drive.Length);
                    var targetPath = Path.Combine(output, basePath);

                    if (IsDirectory(path))
                    {
                        //MSBuild, for whatever reason, behaves differently based on the mere existence of some folders.
                        Directory.CreateDirectory(targetPath);
                    }
                    else if (File.Exists(path))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                        File.Copy(path, targetPath);
                    }
                }
                catch
                {
                    Console.WriteLine(path);
                }
            }
        }

        static bool IsDirectory(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        static IEnumerable<ProcessMonitorEntry> Parse(string path, bool header = true)
        {
            var parser = new TextFieldParser(path);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            if (header)
            {
                parser.ReadLine();
            }
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                yield return new ProcessMonitorEntry
                {
                    //TimeOfDay = fields[0],
                    //ProcessName = fields[1],
                    //PID = fields[2],
                    //Operation = fields[3],
                    Path = fields[4],
                    Result = fields[5],
                    //Details = fields[6]
                };
            }
            parser.Close();
        }
    }

    class ProcessMonitorEntry
    {
        //public string TimeOfDay { get; set; }
        //public string ProcessName { get; set; }
        //public string PID { get; set; }
        //public string Operation { get; set; }
        public string Path { get; set; }
        public string Result { get; set; }
        //public string Details { get; set; }
    }
}
