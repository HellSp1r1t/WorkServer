using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkServer
{
    public class Documents
    {
        public static string GetAllFilesInFolder() {
            string[] dirs = Directory.GetFiles(Environment.CurrentDirectory + "/texts");
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] t = dirs[i].Split('\\');
                dirs[i] = t[t.Length - 1];
            }
            return string.Join(",", dirs);
        }
        public static string GetContentFromFile(string filePath) {
            string[] allLines = File.ReadAllLines(filePath);
            return string.Join("/n", allLines);
        }
        public static void WriteContentToFile(string path, string content) {
            StreamWriter swe = File.CreateText(path);
            swe.WriteLine(content);
            swe.Close();
        }
    }
}
