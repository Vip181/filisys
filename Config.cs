using System;
using System.IO;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
namespace filesys
{
    static class Config
    {
        public const string FileName = @"0:\screen.txt"; // disque 0, racine

        // Sauvegarde la résolution
        public static void initialefile()
        {
            Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            var fs_type = fs.GetFileSystemType(@"0:\");
        }
        public static void SaveResolution(int width, int height)
        {

            try
            {
                string data = width + ";" + height;
                File.WriteAllText(FileName, data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur sauvegarde config: " + e.Message);
            }
        }

        // Charge la résolution si le fichier existe, sinon valeurs par défaut
        public static (int width, int height) LoadResolution()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    string data = File.ReadAllText(FileName);
                    var parts = data.Split(';');
                    if (parts.Length == 2)
                    {
                        int w = int.Parse(parts[0]);
                        int h = int.Parse(parts[1]);
                        return (w, h);
                    }
                }
            }
            catch
            {
                // ignore les erreurs
            }

            return (1920, 1080); // valeur par défaut
        }
    }
}