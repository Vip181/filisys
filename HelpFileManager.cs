using System;
using System.IO;

public static class HelpFileManager
{
    private static string helpFile = @"0:\help.txt";

    private static string[] commands =
    {
        "help","mem","ramfix","clear","res","reboot","shutdown","tasks",
        "ls","cd","cat","touch","edit","rescan"
    };

    public static void EnsureHelpFile()
    {
        try
        {
            // 🔥 SI FICHIER N'EXISTE PAS → CREATE
            if (!File.Exists(helpFile))
            {
                using (var fs = File.Create(helpFile)) { }

                WriteCommands();
                return;
            }

            // 🔥 SI EXISTE → vérifier si vide
            var content = File.ReadAllText(helpFile);

            if (string.IsNullOrWhiteSpace(content))
            {
                WriteCommands();
            }
        }
        catch (Exception)
        {
            // ignore safe (Cosmos OS safe mode)
        }
    }

    private static void WriteCommands()
    {
        try
        {
            using (var sw = new StreamWriter(helpFile, false))
            {
                foreach (var cmd in commands)
                {
                    sw.WriteLine(cmd);
                }
            }
        }
        catch (Exception)
        {
            // safe fail
        }
    }
}