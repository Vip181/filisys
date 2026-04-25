using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;

namespace filesys.GUI
{
    public enum IconType
    {
        File,
        Folder,
        Console,
        TaskManager,
        Custom
    }
    public class DesktopIcon
    {
        public IconType Type;
        public string Name;
        public string Path;
        public bool IsFolder;
        private float scale = 1f;
        private float targetScale = 1f;
        public int X;
        public int Y;

        private int size = 50;

        private bool dragging = false;
        private int offsetX;
        private int offsetY;

        public Action OnOpen;

        public DesktopIcon(string name, string path, IconType type, int x, int y, Action onOpen)
        {
            Name = name;
            Path = path;
            Type = type;
            X = x;
            Y = y;
            OnOpen = onOpen;
        }

        public void Update()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            bool inside =
                mx >= X && mx <= X + 50 &&
                my >= Y && my <= Y + 50;

            // 🎯 HOVER ZOOM TARGET
            targetScale = inside ? 1.2f : 1.0f;

            // 🎬 SMOOTH ANIMATION
            scale += (targetScale - scale) * 0.2f;

            // DRAG LOGIC (inchangé)
            if (click && inside && !dragging)
            {
                dragging = true;
                offsetX = mx - X;
                offsetY = my - Y;
            }

            if (dragging && click)
            {
                X = mx - offsetX;
                Y = my - offsetY;
            }

            if (!click)
            {
                if (dragging && inside)
                    OnOpen?.Invoke();

                dragging = false;
            }
        }

        public void Draw(Canvas canvas)
        {
            int size = 40;

            int drawSize = (int)(size * scale);
            int offset = (drawSize - size) / 2;

            int dx = X - offset;
            int dy = Y - offset;
           

            // ICON (zoom appliqué)
            switch (Type)
            {
                case IconType.Console:
                    UIIcons.Console(canvas, dx, dy);
                    break;

                case IconType.File:
                    UIIcons.File(canvas, dx, dy);
                    break;

                case IconType.Folder:
                    UIIcons.Folder(canvas, dx, dy);
                    break;

                case IconType.TaskManager:
                    UIIcons.TaskManager(canvas, dx, dy);
                    break;
            }

            // label reste fixe
            canvas.DrawString(
                Name,
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                new Pen(Color.White),
                X,
                Y + 55
            );
        }
    }
}