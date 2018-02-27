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
        static void Main(string[] args)
        {
            string key;

            /// Load XML
            string xmlPath = @".\config\config.xml";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(xmlPath));

            /// Get nodes
            XmlNodeList list = doc.DocumentElement.SelectNodes("//Path");
            List<string> zipDirs = new List<string>();

            /// Save paths to list
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
                    /// Parent dir of target dirs goes here
                    string DS = zipDirs.ElementAt(index: i);
                    if (Directory.Exists(DS))
                    {
                        string[] filesArray = Directory.GetDirectories(DS);
                        foreach (string s in filesArray)
                        {
                            /// Check if zip file already exists
                            if (!File.Exists(DS + s + ".zip"))
                            {
                                /// Create zip file
                                ZipFile.CreateFromDirectory(DS + s, DS + s + ".zip");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogging(ex);
                    ReadError();
                }
            }
        }

        /// Error Handling
        public static void ErrorLogging(Exception ex)
        {
            string strPath = @".\log\Log.txt";
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);

            }
        }

        public static void ReadError()
        {
            string strPath = @"\log\Log.txt";
            using (StreamReader sr = new StreamReader(strPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
