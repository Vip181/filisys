using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System.Collections.Generic;
using System.IO;

namespace filesys.System
{
    public static class FileSystemHelper
    {
        public static string Root = @"0:\";

        public static List<string> GetDirectories(string path)
        {
            List<string> dirs = new List<string>();
            try
            {
                foreach (var d in Directory.GetDirectories(path))
                    dirs.Add(d);
            }
            catch { }
            return dirs;
        }

        public static List<string> GetFiles(string path)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (var f in Directory.GetFiles(path))
                    files.Add(f);
            }
            catch { }
            return files;
        }
    }
}