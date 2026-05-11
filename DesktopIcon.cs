using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;

namespace filesys.GUI
{
    public enum IconType
    {
        File,
        Folder,
        Back
    }

    public class DesktopIcon
    {
        public string Name;
        public string Path;
        public IconType Type;
        public int X;
        public int Y;

        private Action onClick;
        private bool lastClick = false;

        private const int Width = 70;
        private const int Height = 70;

        private static Pen White = new Pen(Color.White);
        private static Pen Black = new Pen(Color.Black);
        private static Pen FolderColor = new Pen(Color.Goldenrod);
        private static Pen FileColor = new Pen(Color.LightGray);
        private static Pen BackColor = new Pen(Color.LightBlue);

        public DesktopIcon(string name, string path, IconType type, int x, int y, Action clickAction)
        {
            Name = name;
            Path = path;
            Type = type;
            X = x;
            Y = y;
            onClick = clickAction;
        }

        public void Update()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            bool click = MouseManager.MouseState == MouseState.Left;

            bool hover =
                mx >= X &&
                mx <= X + Width &&
                my >= Y &&
                my <= Y + Height;

            if (click && !lastClick && hover)
            {
                if (onClick != null)
                    onClick();
            }

            lastClick = click;
        }

        public void Draw(Canvas canvas)
        {
            if (canvas == null)
                return;

            if (Type == IconType.Folder)
            {
                // Même icône que le menu démarrer
                if (StartMenuIcons.Explorer != null)
                    canvas.DrawImageAlpha(StartMenuIcons.Explorer, X + 18, Y + 6);
            }
            else if (Type == IconType.File)
            {
                if (StartMenuIcons.Console != null)
                    canvas.DrawImageAlpha(StartMenuIcons.TEXTDCOCUMENT, X + 18, Y + 6);
            }
            else if (Type == IconType.Back)
            {
                canvas.DrawFilledRectangle(BackColor, X + 15, Y + 15, 40, 30);
                canvas.DrawString("<", PCScreenFont.Default, Black, X + 30, Y + 25);
            }

            string label = Name;

            if (label.Length > 10)
                label = label.Substring(0, 10);

            canvas.DrawString(label, PCScreenFont.Default, White, X, Y + 58);
        }
    }
}