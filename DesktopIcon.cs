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
        Custom,
        Back // nouvel icone "Retour"
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

        // nouveau état pour la détection correcte du clic (pression puis relâche)
        private bool pressedInside = false;

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
            bool mouseDown = MouseManager.MouseState == MouseState.Left;

            // calculer la zone effectivement dessinée (prend en compte le zoom)
            int baseSize = 40;
            int drawSize = (int)(baseSize * scale);
            int offset = (drawSize - baseSize) / 2;
            int drawX = X - offset;
            int drawY = Y - offset;

            bool inside =
                mx >= drawX && mx <= drawX + drawSize &&
                my >= drawY && my <= drawY + drawSize;

            // HOVER ZOOM TARGET
            targetScale = inside ? 1.2f : 1.0f;

            // SMOOTH ANIMATION
            scale += (targetScale - scale) * 0.2f;

            // GESTION DU DRAG
            if (mouseDown)
            {
                if (inside && !dragging)
                {
                    dragging = true;
                    offsetX = mx - X;
                    offsetY = my - Y;
                }

                if (dragging)
                {
                    X = mx - offsetX;
                    Y = my - offsetY;
                }

                // mémoriser qu'on a pressé à l'intérieur (pour valider un clic au relâchement)
                if (inside)
                    pressedInside = true;
            }
            else
            {
                // relâchement : si on avait pressé à l'intérieur et on relâche toujours à l'intérieur => ouvrir
                if (pressedInside && inside)
                {
                    OnOpen?.Invoke();
                }

                // reset états
                pressedInside = false;
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

                case IconType.Back:
                    UIIcons.Back(canvas, dx, dy);
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