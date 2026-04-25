using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.IO;
using System.Drawing;

namespace filesys.GUI
{
    public class FileExplorerWindow : BaseWindow
    {
        private string[] files;

        public FileExplorerWindow(int x, int y)
            : base("Files", x, y, 300, 300)
        {
            Refresh();
        }

        private void Refresh()
        {
            try { files = Directory.GetFiles(@"0:\"); }
            catch { files = new string[0]; }
        }

        public override void Update()
        {
            base.Update();
            if (IsMinimized || IsClosed) return;

            if (MouseManager.MouseState == MouseState.Left)
            {
                int mx = (int)MouseManager.X;
                int my = (int)MouseManager.Y;

                int y = Y + 35;
                foreach (var f in files)
                {
                    if (my >= y && my <= y + 18 &&
                        mx >= X + 10 && mx <= X + Width - 10)
                    {
                        Kernel.Instance.AddWindow(
                            new FileViewer(Path.GetFileName(f), X + 40, Y + 40)
                        );
                        break;
                    }
                    y += 18;
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized || IsClosed) return;
            base.Draw(canvas);

            int y = Y + 35;
            foreach (var f in files)
            {
                canvas.DrawString(
                    Path.GetFileName(f),
                    PCScreenFont.Default,
                    new Pen(Color.Beige),
                    X + 10,
                    y
                );
                y += 18;
            }
        }
    }
}
