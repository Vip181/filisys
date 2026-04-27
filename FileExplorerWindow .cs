using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.IO;
using System.Drawing;

namespace filesys.GUI
{
    public class FileExplorerWindow : BaseWindow
    {
        // 📁 navigation
        private string currentPath = @"0:\";
        private string[] files = new string[0];
        private string[] directories = new string[0];

        // 🌳 tree / 📄 file scroll
        private int treeScroll = 0;
        private int fileScroll = 0;

        // sélection fichier
        private int selectedFile = -1;

        private const int treeWidth = 140;
        private const int itemHeight = 18;

        public FileExplorerWindow(int x, int y)
            : base("Explorer", x, y, 520, 360)
        {
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                files = Directory.GetFiles(currentPath);
                directories = Directory.GetDirectories(currentPath);
            }
            catch
            {
                files = new string[0];
                directories = new string[0];
            }
        }

        public override void Update()
        {
            base.Update();
            if (IsMinimized || IsClosed) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int startY = Y + 30;

            // =========================
            // 🌳 CLICK TREEVIEW
            // =========================
            if (MouseManager.MouseState == MouseState.Left)
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    int y = startY + (i - treeScroll) * itemHeight;

                    if (y < startY || y > Y + Height)
                        continue;

                    if (mx >= X && mx <= X + treeWidth &&
                        my >= y && my <= y + itemHeight)
                    {
                        currentPath = directories[i];
                        selectedFile = -1;
                        treeScroll = 0;
                        fileScroll = 0;
                        Refresh();
                        return;
                    }
                }
            }

            // =========================
            // 📄 CLICK FILE PANEL
            // =========================
            int fileX = X + treeWidth + 5;

            if (MouseManager.MouseState == MouseState.Left)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    int y = startY + (i - fileScroll) * itemHeight;

                    if (y < startY || y > Y + Height)
                        continue;

                    if (mx >= fileX && mx <= X + Width &&
                        my >= y && my <= y + itemHeight)
                    {
                        selectedFile = i;

                        Kernel.Instance.AddWindow(
                            new FileViewer(Path.GetFileName(files[i]), X + 60, Y + 60)
                        );
                        return;
                    }
                }
            }

            // =========================
            // 🔽 SCROLL SIMULÉ (COSMOS SAFE)
            // =========================
            if (MouseManager.MouseState == MouseState.Left)
            {
                // zone droite = scroll down
                if (mx > X + Width - 15 && my > startY)
                {
                    fileScroll++;
                    treeScroll++;
                }
            }

            if (MouseManager.MouseState == MouseState.Right)
            {
                // scroll up
                if (mx > X + Width - 15 && my > startY)
                {
                    fileScroll--;
                    treeScroll--;

                    if (fileScroll < 0) fileScroll = 0;
                    if (treeScroll < 0) treeScroll = 0;
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized || IsClosed) return;
            base.Draw(canvas);

            int startY = Y + 30;

            // =========================
            // 🌳 TREE PANEL BACKGROUND
            // =========================
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(35, 35, 35)),
                X,
                startY,
                treeWidth,
                Height - 30
            );

            // =========================
            // 📄 FILE PANEL BACKGROUND
            // =========================
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(20, 20, 20)),
                X + treeWidth + 5,
                startY,
                Width - treeWidth - 5,
                Height - 30
            );

            // =========================
            // 🌳 DRAW DIRECTORIES
            // =========================
            for (int i = 0; i < directories.Length; i++)
            {
                int y = startY + (i - treeScroll) * itemHeight;

                if (y < startY || y > Y + Height)
                    continue;

                string name = Path.GetFileName(directories[i]);

                canvas.DrawString(
                    "📁 " + name,
                    PCScreenFont.Default,
                    new Pen(Color.LightGreen),
                    X + 5,
                    y
                );
            }

            // =========================
            // 📄 DRAW FILES
            // =========================
            for (int i = 0; i < files.Length; i++)
            {
                int y = startY + (i - fileScroll) * itemHeight;

                if (y < startY || y > Y + Height)
                    continue;

                string name = Path.GetFileName(files[i]);

                // sélection highlight
                if (i == selectedFile)
                {
                    canvas.DrawFilledRectangle(
                        new Pen(Color.FromArgb(80, 80, 140)),
                        X + treeWidth + 5,
                        y,
                        Width - treeWidth - 5,
                        itemHeight
                    );
                }

                DrawFileIcon(canvas, X + treeWidth + 8, y + 3);

                canvas.DrawString(
                    name,
                    PCScreenFont.Default,
                    new Pen(Color.White),
                    X + treeWidth + 25,
                    y + 3
                );
            }

            // =========================
            // DIVIDER
            // =========================
            canvas.DrawLine(
                new Pen(Color.Gray),
                X + treeWidth,
                startY,
                X + treeWidth,
                Y + Height
            );

            // =========================
            // PATH BAR
            // =========================
            canvas.DrawString(
                currentPath,
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 5,
                Y + 5
            );

            // =========================
            // SCROLL HINT BAR
            // =========================
            canvas.DrawFilledRectangle(
                new Pen(Color.DarkGray),
                X + Width - 10,
                startY,
                4,
                Height - 30
            );
        }

        // 📄 icône fichier
        private void DrawFileIcon(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(
                new Pen(Color.LightGray),
                x,
                y,
                10,
                10
            );

            canvas.DrawRectangle(
                new Pen(Color.White),
                x,
                y,
                10,
                10
            );
        }
    }
}