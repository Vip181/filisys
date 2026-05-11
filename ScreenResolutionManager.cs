using System;
using System.IO;

namespace filesys.System
{
    public static class ScreenResolutionManager
    {
        private const string ResFile = @"0:\screen_resolution.cfg";

        // ================= SAVE =================
        public static void SaveResolution(int index)
        {
            try
            {
                File.WriteAllText(ResFile, index.ToString());
            }
            catch
            {
                // silencieux (Cosmos-safe)
            }
        }

        // ================= LOAD INDEX =================
        public static int LoadSavedResolutionIndex()
        {
            try
            {
                if (!File.Exists(ResFile))
                    return 0;

                string content = File.ReadAllText(ResFile);

                int index;
                if (int.TryParse(content, out index))
                    return index;
            }
            catch { }

            return 0;
        }

        // ================= LOAD WIDTH / HEIGHT =================
        public static void LoadResolution(out int width, out int height)
        {
            // valeurs par défaut SAFE
            width = 1024;
            height = 768;

            try
            {
                if (!File.Exists(ResFile))
                    return;

                string[] p = File.ReadAllText(ResFile).Split(';');

                if (p.Length >= 2)
                {
                    width = int.Parse(p[0]);
                    height = int.Parse(p[1]);
                }
            }
            catch
            {
                width = 1024;
                height = 768;
            }
        }
    }
}