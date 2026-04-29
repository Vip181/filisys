using System;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace filesys.GUI
{
    public class BaseWindow
    {
        public int X, Y, Width, Height;
        public string Title;
        public bool IsClosed = false;
        public bool IsMinimized = false;

        protected bool dragging = false;
        protected bool resizing = false;
        protected int offsetX, offsetY;

        protected const int TitleBarHeight = 30;
        protected const int ResizeSize = 10;

        public BaseWindow(string title, int x, int y, int w, int h)
        {
            Title = title;
            X = x; Y = y; Width = w; Height = h;
        }

        public virtual void Update()
        {
            if (IsClosed) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool pressed = MouseManager.MouseState == MouseState.Left;

            // Fermer
            if (pressed &&
                mx >= X + Width - 25 && mx <= X + Width - 5 &&
                my >= Y + 5 && my <= Y + 25)
            {
                IsClosed = true;
                return;
            }

            // Réduire
            if (pressed &&
                mx >= X + Width - 50 && mx <= X + Width - 30 &&
                my >= Y + 5 && my <= Y + 25)
            {
                IsMinimized = true;
                return;
            }

            // Resize (coin bas droit)
            if (pressed &&
                mx >= X + Width - ResizeSize &&
                my >= Y + Height - ResizeSize)
            {
                resizing = true;
            }

            // Drag
            if (pressed && !dragging && !resizing &&
                mx >= X && mx <= X + Width - 60 &&
                my >= Y && my <= Y + TitleBarHeight)
            {
                dragging = true;
                offsetX = mx - X;
                offsetY = my - Y;
            }

            if (!pressed)
            {
                dragging = false;
                resizing = false;
            }

            if (dragging)
            {
                X = mx - offsetX;
                Y = my - offsetY;
            }

            if (resizing)
            {
                Width = Math.Max(200, mx - X);
                Height = Math.Max(150, my - Y);
            }
        }

        public virtual void Draw(Canvas canvas)
        {
            if (IsMinimized || IsClosed) return;

            canvas.DrawFilledRectangle(StyleManager.WindowBg, X, Y, Width, Height);
            canvas.DrawFilledRectangle(StyleManager.TitleBar, X, Y, Width, TitleBarHeight);

            canvas.DrawString(Title, PCScreenFont.Default, StyleManager.TextWhite, X + 10, Y + 8);

            canvas.DrawString("_", PCScreenFont.Default, StyleManager.TextWhite, X + Width - 45, Y + 5);
            canvas.DrawString("X", PCScreenFont.Default, StyleManager.CloseButton, X + Width - 20, Y + 8);

            // Coin resize visuel
            canvas.DrawString("◢", PCScreenFont.Default, StyleManager.TextWhite,
                X + Width - 12, Y + Height - 14);
        }
    }
}