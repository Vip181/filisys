using System;
using System.IO;
using System.Text;

namespace filesys.Cache
{
    public static class DiskRamCache
    {
        // CONFIG
        private const int MAX_FILES = 8;        // nombre max de fichiers en cache
        private const int MAX_FILE_SIZE = 8192; // 8 KB par fichier

        private static CacheEntry[] entries = new CacheEntry[MAX_FILES];
        private static int index = 0;

        static DiskRamCache()
        {
            for (int i = 0; i < MAX_FILES; i++)
                entries[i] = new CacheEntry();
        }

        // ===== LECTURE =====
        public static bool TryReadText(string path, StringBuilder output)
        {
            // 1) chercher dans le cache
            for (int i = 0; i < MAX_FILES; i++)
            {
                if (entries[i].Used && entries[i].Path == path)
                {
                    output.Clear();
                    output.Append(entries[i].Buffer);
                    return true; // HIT
                }
            }

            // 2) pas en cache → lire disque
            if (!File.Exists(path))
                return false;

            using (var fs = new FileStream(path, FileMode.Open))
            {
                if (fs.Length > MAX_FILE_SIZE)
                    return false; // trop gros → pas en cache

                using (var sr = new StreamReader(fs))
                {
                    output.Clear();
                    output.Append(sr.ReadToEnd());
                }
            }

            // 3) stocker en cache
            Store(path, output);

            return true;
        }

        // ===== ÉCRITURE =====
        public static void WriteText(string path, StringBuilder content)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(content.ToString());
            }

            // mettre à jour cache si présent
            for (int i = 0; i < MAX_FILES; i++)
            {
                if (entries[i].Used && entries[i].Path == path)
                {
                    entries[i].Buffer.Clear();
                    entries[i].Buffer.Append(content);
                    return;
                }
            }
        }

        // ===== STOCKAGE =====
        private static void Store(string path, StringBuilder data)
        {
            CacheEntry e = entries[index];

            e.Used = true;
            e.Path = path;
            e.Buffer.Clear();
            e.Buffer.Append(data);

            index++;
            if (index >= MAX_FILES)
                index = 0;
        }

        // ===== STRUCTURE =====
        private class CacheEntry
        {
            public bool Used;
            public string Path;
            public StringBuilder Buffer = new StringBuilder(MAX_FILE_SIZE);
        }
    }
}