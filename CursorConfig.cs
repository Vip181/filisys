using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using System.IO;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;

namespace filesys
{
    public static class CursorConfig
    {
        public const string FileName = @"0:\cursor.cfg";

        public static void Save(CursorStyle style)
        {
            File.WriteAllText(FileName, style.ToString());
        }

        public static CursorStyle Load()
        {
            
            try
            {
                if (File.Exists(FileName))
                {
                    string txt = File.ReadAllText(FileName);

                    if (txt == "Arrow") return CursorStyle.Arrow;
                    if (txt == "Cross") return CursorStyle.Cross;
                    if (txt == "Block") return CursorStyle.Block;
                }
            }
            catch { }

            return CursorStyle.Arrow;
        }
    }
}
