using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace cadgrptools
{
    public class ConfigFileRW
    {




        public static string GetDLLFilePath(string fileName)
        {
            // Lấy đường dẫn của tệp DLL đang thực thi
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            return Path.Combine(filePath, fileName);
        }

        public static Dictionary<string, string> ReadKeyValueFromFile(string filePath)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Tách key và value từ dòng văn bản
                        string[] parts = line.Split(':');
                        if (parts.Length == 2) // Only one ':' in line
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            data[key] = value;
                        }
                    }
                }

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }

        }




        private static void WriteKeyValueToFile(string fileName, Dictionary<string, string> data)
        {
            //string directory = Directory.GetCurrentDirectory();
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            string filePath = Path.Combine(directory, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var kvp in data)
                {
                    writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }

    }
}
