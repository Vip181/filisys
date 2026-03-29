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
        protected int offsetX, offsetY;

        public BaseWindow(string title, int x, int y, int w, int h)
        {
            Title = title;
            X = x; Y = y; Width = w; Height = h;
        }

        public virtual void Update()
        {
            if (IsMinimized) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool pressed = MouseManager.MouseState == MouseState.Left;

            if (pressed)
            {
                // Dragging via la barre de titre (30px)
                if (!dragging && mx >= X && mx <= X + Width && my >= Y && my <= Y + 30)
                {
                    dragging = true;
                    offsetX = mx - X;
                    offsetY = my - Y;
                }
            }
            else dragging = false;

            if (dragging)
            {
                X = mx - offsetX;
                Y = my - offsetY;
            }

            // Clic sur bouton fermer
            if (pressed && !dragging && mx >= X + Width - 30 && mx <= X + Width && my >= Y && my <= Y + 30)
            {
                IsClosed = true;
            }
        }

        public virtual void Draw(Canvas canvas)
        {
            if (IsMinimized) return;

            canvas.DrawFilledRectangle(StyleManager.WindowBg, X, Y, Width, Height);
            canvas.DrawFilledRectangle(StyleManager.TitleBar, X, Y, Width, 30);
            canvas.DrawString(Title, PCScreenFont.Default, StyleManager.TextWhite, X + 10, Y + 8);
            canvas.DrawString("X", PCScreenFont.Default, StyleManager.CloseButton, X + Width - 20, Y + 8);
        }
    }
}