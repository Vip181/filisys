using Cosmos.System.Graphics;
using filesys.System;
using System.Collections.Generic;
using System.IO;

namespace filesys.GUI
{
    public class DesktopManager
    {
        public List<DesktopIcon> Icons = new List<DesktopIcon>();

        private string currentPath = @"0:\";

        public void Refresh()
        {
            Icons.Clear();

            int x = 100;
            int y = 100;

            // 📁 dossiers
            foreach (var dir in FileSystemHelper.GetDirectories(currentPath))
            {
                string name = Path.GetFileName(dir);

                Icons.Add(new DesktopIcon(
                    name,
                    dir,
                    IconType.Folder,
                    x, y,
                    () => ChangeDirectory(dir)
                ));

                x += 70;
            }

            // 📄 fichiers
            foreach (var file in FileSystemHelper.GetFiles(currentPath))
            {
                string name = Path.GetFileName(file);

                Icons.Add(new DesktopIcon(
                    name,
                    file,
                    IconType.File,
                    x, y,
                    () => OpenFile(file)
                ));

                x += 70;
            }
        }

        public void ChangeDirectory(string path)
        {
            currentPath = path;
            Refresh();
        }

        public void OpenFile(string file)
        {
            // 🔥 simple test
            Kernel.Instance.AddWindow(new WindowsConsole(200, 200));
        }

        public void Update()
        {
            foreach (var i in Icons)
                i.Update();
        }

        public void Draw(Canvas canvas)
        {
            int startX = 40;
            int startY = 40;

            int spacingX = 90;
            int spacingY = 90;

            int maxPerRow = 6;

            int i = 0;

            foreach (var icon in Icons)
            {
                int col = i % maxPerRow;
                int row = i / maxPerRow;

                icon.X = startX + col * spacingX;
                icon.Y = startY + row * spacingY;

                icon.Draw(canvas);
                i++;
            }
        }
    }
}