using System.Collections.Generic;
using System.IO;

namespace PinyinAnalyzer
{
    public static class Path
    {
        public static IEnumerable<string> GetFiles(string pathPattern)
        {
            var fileinfo = new FileInfo(pathPattern);
            return Directory.GetFiles(fileinfo.DirectoryName, fileinfo.Name);
        }

        public static string ChangeDir (string path, string dir)
        {
            return dir + new FileInfo(path).Name;
        }
    }
}