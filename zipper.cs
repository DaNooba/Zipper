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
        // Path to error log
        const string strPath = @".\log\Log.txt";
        // Load lazy streamwriter
        static Lazy<StreamWriter> logWriter = new Lazy<StreamWriter>(() => File.CreateText(strPath)); 

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

            for (int i = 0; i < zipDirs.Count; i++)
            {
                try
                {
                    // Parent dir of target dirs goes here
                    string DS = zipDirs.ElementAt(index: i);
                    if (Directory.Exists(DS))
                    {
                        string[] filesArray = Directory.GetDirectories(DS);
                        Parallel.ForEach(filesArray, s =>
                        {
                            var path = Path.Combine(DS, s);
                            var zipPath = path + ".zip";

                            // Check if zip file already exists
                            if (!File.Exists(zipPath))
                            {
                                // Create zip file
                                ZipFile.CreateFromDirectory(path, zipPath);
                            }
                            else
                            {
                                // Check if the folder has been modified since 
                                if (CheckFiles(path, zipPath))
                                {
                                    File.Delete(zipPath);
                                    ZipFile.CreateFromDirectory(path, zipPath);
                                }
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

        /// <summary>
        /// Checks folder against dir, returns true if folder is newer
        /// </summary>
        /// <param name="path"></param>
        /// <param name="zipPath"></param>
        /// <returns></returns>
        public static bool CheckFiles(String path, String zipPath)
        {
            var t1 = Directory.GetLastWriteTime(path);
            var t2 = File.GetCreationTime(zipPath);

            if (DateTime.Compare(t1, t2) > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Error Handling:
        /// Writes error to specified file
        /// </summary>
        /// <param name="ex"></param>
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