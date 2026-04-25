using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.IO;
using System.Drawing;
using filesys.System;

namespace filesys.GUI
{
    public class FileViewer : BaseWindow
    {
        private string filePath;
        private string fileContent = "";
        private Button saveButton;
        private bool isModified = false;

        public FileViewer(string filename, int x, int y) : base("Editor: " + filename, x, y, 400, 350)
        {
            this.filePath = @"0:\" + filename;

            // Bouton de sauvegarde en bas de la fenêtre
            saveButton = new Button(X + Width - 80, Y + Height - 25, 70, 20, "Save", SaveFile);

            LoadFileSafe();
        }

        private void LoadFileSafe()
        {
            try
            {
                if (File.Exists(filePath)) fileContent = File.ReadAllText(filePath);
            }
            catch { fileContent = ""; }
        }

        private void SaveFile()
        {
            try
            {
                File.WriteAllText(filePath, fileContent);
                isModified = false;
                Title = "Editor: " + Path.GetFileName(filePath);
            }
            catch { /* Gérer erreur */ }
        }

        public override void Update()
        {
            base.Update();
            if (IsMinimized || IsClosed) return;

            // Mise à jour de la position du bouton si on déplace la fenêtre
            saveButton.X = X + Width - 80;
            saveButton.Y = Y + Height - 25;
            saveButton.Update();

            // GESTION DU CLAVIER (Uniquement si cette fenêtre est active/focus)
            // Note: Dans le Kernel, vous devrez vérifier si cette fenêtre est celle du dessus
            KeyEvent key;
            if (KeyboardManager.TryReadKey(out key))
            {
                HandleKeyboard(key);
            }
        }

        private void HandleKeyboard(KeyEvent key)
        {
            if (key.Key == ConsoleKeyEx.Backspace)
            {
                if (fileContent.Length > 0)
                    fileContent = fileContent.Substring(0, fileContent.Length - 1);
            }
            else if (key.Key == ConsoleKeyEx.Enter)
            {
                fileContent += "\n";
            }
            else if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || key.KeyChar == ' ')
            {
                fileContent += key.KeyChar;
            }

            isModified = true;
            Title = "Editor: " + Path.GetFileName(filePath) + "*";
        }

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized || IsClosed) return;
            base.Draw(canvas);

            // Zone d'édition
            canvas.DrawFilledRectangle(new Pen(Color.White), X + 5, Y + 35, Width - 10, Height - 65);

            // Affichage du texte
            string[] lines = fileContent.Split('\n');
            int yOffset = Y + 40;
            foreach (var line in lines)
            {
                if (yOffset > Y + Height - 40) break;
                canvas.DrawString(line, PCScreenFont.Default, new Pen(Color.Black), X + 10, yOffset);
                yOffset += 16;
            }

            // Dessiner le bouton Save
            saveButton.Draw(canvas);
        }
    }
}