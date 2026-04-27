using Cosmos.System.Graphics.Fonts;
using System.Drawing;
using System.IO;

public static class UISettings
{
    public static PCScreenFont CurrentFont = PCScreenFont.Default;
    public static Color TextColor = Color.White;

    private static string configFile = @"0:\system\ui.cfg";

    public static void Load()
    {
        if (!File.Exists(configFile))
        {
            Save();
            return;
        }

        foreach (var line in File.ReadAllLines(configFile))
        {
            if (line.StartsWith("FONT="))
            {
                string f = line.Substring(5);
                CurrentFont = PCScreenFont.Default;
            }

            if (line.StartsWith("COLOR="))
            {
                var p = line.Substring(6).Split(',');
                TextColor = Color.FromArgb(
                    int.Parse(p[0]),
                    int.Parse(p[1]),
                    int.Parse(p[2])
                );
            }
        }
    }

    public static void Save()
    {
        Directory.CreateDirectory(@"0:\system\");

        File.WriteAllLines(configFile, new string[]
        {
            "FONT=DEFAULT",
            $"COLOR={TextColor.R},{TextColor.G},{TextColor.B}"
        });
    }
}