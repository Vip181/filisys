using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
namespace filesys.GUI
{
    public class FileViewer : BaseWindow
    {
        private string filePath;
        private string fileContent = "";

        private Button saveButton;
        private Button deleteButton;

        private bool isModified = false;
        private bool confirmDelete = false;

        public FileViewer(string filename, int x, int y)
            : base("Editor: " + Path.GetFileName(filename), x, y, 400, 350)
        {
            // Utiliser le chemin fourni
            filePath = filename;

            saveButton = new Button(
                X + Width - 80,
                Y + Height - 25,
                70,
                20,
                "Save",
                SaveFile
            );

            deleteButton = new Button(
                X + 10,
                Y + Height - 25,
                70,
                20,
                "Delete",
                AskDelete
            );

            LoadFile();
        }

        private void LoadFile()
        {
            try
            {
                // Tentative 1 : chemin tel quel
                if (File.Exists(filePath))
                {
                    fileContent = File.ReadAllText(filePath);
                    if (string.IsNullOrEmpty(fileContent))
                        fileContent = "[Fichier vide]";
                    return;
                }

                // Tentative 2 : supprimer doublons "0:\0:\"
                var alt = filePath;
                while (alt.StartsWith(@"0:\0:\"))
                    alt = alt.Substring(3);
                if (alt != filePath && File.Exists(alt))
                {
                    fileContent = File.ReadAllText(alt);
                    if (string.IsNullOrEmpty(fileContent))
                        fileContent = "[Fichier vide]";
                    filePath = alt; // mettre à jour pour affichage
                    return;
                }

                // Tentative 3 : remplacer slash avant/backslash
                var alt2 = filePath.Replace('\\', '/');
                if (File.Exists(alt2))
                {
                    fileContent = File.ReadAllText(alt2);
                    if (string.IsNullOrEmpty(fileContent))
                        fileContent = "[Fichier vide]";
                    filePath = alt2;
                    return;
                }

                // Tentative 4 : essayer avec un préfixe "0:\" si absent
                if (!filePath.StartsWith(@"0:\"))
                {
                    var pref = @"0:\" + filePath.TrimStart('\\', '/');
                    if (File.Exists(pref))
                    {
                        fileContent = File.ReadAllText(pref);
                        if (string.IsNullOrEmpty(fileContent))
                            fileContent = "[Fichier vide]";
                        filePath = pref;
                        return;
                    }
                }

                // Aucun succès → message explicite
                fileContent = "[File not found] " + filePath;
            }
            catch (Exception ex)
            {
                fileContent = "[Error reading file] " + ex.Message;
            }
        }

        private void SaveFile()
        {
            try
            {
                File.WriteAllText(filePath, fileContent);
                ExecuteDelte();
                isModified = false;
                Title = "Editor: " + Path.GetFileName(filePath);
            }
            catch { }
        }

        private void ExecuteDelte()
        {
            string del = @"0:\delte";
            if (!File.Exists(del)) return;

            try
            {
                string cmd = File.ReadAllText(del).Trim();
                if (cmd.StartsWith("delete file "))
                {
                    string target = cmd.Replace("delete file ", "");
                    string path = @"0:\" + target;
                    if (File.Exists(path)) File.Delete(path);
                }
                File.Delete(del);
            }
            catch { }
        }

        private void AskDelete()
        {
            if (!confirmDelete)
            {
                confirmDelete = true;
                return;
            }

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                IsClosed = true;
            }
            catch { }
        }

        public override void Update()
        {
            base.Update();
            if (IsMinimized || IsClosed) return;

            saveButton.X = X + Width - 80;
            saveButton.Y = Y + Height - 25;
            deleteButton.X = X + 10;
            deleteButton.Y = Y + Height - 25;

            saveButton.Update();
            deleteButton.Update();

            if (KeyboardManager.TryReadKey(out var key))
            {
                if (key.Key == ConsoleKeyEx.Backspace && fileContent.Length > 0)
                    fileContent = fileContent.Substring(0, fileContent.Length - 1);
                else if (key.Key == ConsoleKeyEx.Enter)
                    fileContent += "\n";
                else if (key.KeyChar >= 32)
                    fileContent += key.KeyChar;

                isModified = true;
                Title = "Editor: " + Path.GetFileName(filePath) + "*";
            }
        }
        private string[] WrapText(string text, int maxWidth)
{
    int charWidth = 8; // PCScreenFont ≈ 8px
    int maxChars = Math.Max(1, maxWidth / charWidth);

    string[] words = text.Replace("\r", "").Split(' ');
    string result = "";
    string line = "";

    for (int i = 0; i < words.Length; i++)
    {
        if ((line + words[i]).Length > maxChars)
        {
            result += line + "\n";
            line = words[i] + " ";
        }
        else
        {
            line += words[i] + " ";
        }
    }

    result += line;
    return result.Split('\n');
}
        public override void Draw(Canvas canvas)
        {
            if (IsMinimized || IsClosed) return;
            base.Draw(canvas);

            // Afficher le chemin du fichier en haut pour debug
            canvas.DrawString(
                filePath,
                PCScreenFont.Default,
                new Pen(Color.LightGray),
                X + 10,
                Y + 18
            );

            canvas.DrawFilledRectangle(
                new Pen(Color.White),
                X + 5,
                Y + 35,
                Width - 10,
                Height - 65
            );

            int textX = X + 10;
            int textY = Y + 40;
            int textWidth = Width - 20;
            int textHeight = Height - 70;

            int y = textY;

            foreach (var paragraph in fileContent.Split('\n'))
            {
                string[] lines = WrapText(paragraph, textWidth);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (y > textY + textHeight - 16)
                        break; // stop si plus de place

                    canvas.DrawString(
                        lines[i],
                        PCScreenFont.Default,
                        new Pen(Color.Black),
                        textX,
                        y
                    );

                    y += 16;
                }
            }

            saveButton.Draw(canvas);
            deleteButton.Draw(canvas);

            if (confirmDelete)
            {
                canvas.DrawString(
                    "Click Delete again to confirm",
                    PCScreenFont.Default,
                    new Pen(Color.Red),
                    X + 90,
                    Y + Height - 22
                );
            }
        }
    }
}