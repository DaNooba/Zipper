using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace DOC_ZIP
{
    class Program
    {
        const string strPath = @".\log\Log.txt";
        static Lazy<StreamWriter> logWriter = new Lazy<StreamWriter>(() => File.CreateText(strPath)); // oooooooooh, ooooooooooooooooooooooooooooooooooh

        static void Main(string[] args)
        {
            string key;

            // Load XML
            string xmlPath = @".\config\config.xml";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(xmlPath));

            // Get nodes
            XmlNodeList list = doc.DocumentElement.SelectNodes("//Path");
            List<string> zipDirs = new List<string>();

            // Save paths to list
            foreach (XmlNode node in list)
            {
                key = node.SelectSingleNode("dir").InnerText;

                if (!zipDirs.Contains(key) && !string.IsNullOrEmpty(key))
                {
                    zipDirs.Add(key);
                }
            }

            Console.WriteLine(zipDirs.Count);

            for (int i = 0; i < zipDirs.Count; i++)
            {
                try
                {
                    // Parent dir of target dirs goes here
                    string DS = zipDirs.ElementAt(index: i);
                    if (Directory.Exists(DS))
                    {
                        Console.WriteLine(DS);

                        string[] filesArray = Directory.GetDirectories(DS);
                        Parallel.ForEach(filesArray, s =>
                        {
                            var path = Path.Combine(DS, s);
                            var zipPath = path + ".zip";
                            Console.WriteLine(path);
                            Console.WriteLine(zipPath);

                            // Check if zip file already exists
                            if (!File.Exists(zipPath))
                            {
                                // Create zip file
                                ZipFile.CreateFromDirectory(path, zipPath);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex, Console.Out);
                    ErrorLogging(ex);
                }
            }
        }

        public static bool CheckFiles(DirectoryInfo info, FileInfo fileInfo)
        {
            foreach (FileInfo file in info.EnumerateFileSystemInfos())
            {
                if (file.LastWriteTime.Ticks > fileInfo.LastWriteTime.Ticks)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Error Handling
        /// </summary>
        public static void ErrorLogging(Exception ex)
        {
            WriteException(ex, logWriter.Value);
        }

        public static void WriteException(Exception ex, TextWriter sw)
        {
            sw.WriteLine("=============Error Logging ===========");
            sw.WriteLine($"===========Start============= {DateTime.Now}");
            sw.WriteLine($"Error Message: {ex.Message}");
            sw.WriteLine($"Stack Trace: {ex.StackTrace}");
            sw.WriteLine("===========End============= ");
        }
    }
}
