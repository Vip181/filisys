using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.IO;
using System.Drawing;

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
            : base("Editor: " + filename, x, y, 400, 350)
        {
            filePath = @"0:\" + filename;

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
                if (File.Exists(filePath))
                    fileContent = File.ReadAllText(filePath);
            }
            catch { fileContent = ""; }
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

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized || IsClosed) return;
            base.Draw(canvas);

            canvas.DrawFilledRectangle(
                new Pen(Color.White),
                X + 5,
                Y + 35,
                Width - 10,
                Height - 65
            );

            int y = Y + 40;
            foreach (var line in fileContent.Split('\n'))
            {
                if (y > Y + Height - 40) break;
                canvas.DrawString(
                    line,
                    PCScreenFont.Default,
                    new Pen(Color.Black),
                    X + 10,
                    y
                );
                y += 16;
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