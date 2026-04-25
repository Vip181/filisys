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
            if (IsClosed) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool pressed = MouseManager.MouseState == MouseState.Left;

            // Logique de la barre de titre (toujours active pour pouvoir déplacer/réduire/fermer)
            if (pressed)
            {
                // Clic sur Fermer (X)
                if (mx >= X + Width - 25 && mx <= X + Width - 5 && my >= Y + 5 && my <= Y + 25)
                {
                    IsClosed = true;
                    return;
                }

                // Clic sur Réduire (_)
                if (mx >= X + Width - 50 && mx <= X + Width - 30 && my >= Y + 5 && my <= Y + 25)
                {
                    IsMinimized = true;
                    return;
                }

                // Dragging
                if (!dragging && mx >= X && mx <= X + Width - 60 && my >= Y && my <= Y + 30)
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
        }

        public virtual void Draw(Canvas canvas)
        {
            if (IsMinimized || IsClosed) return;

            // Fond et bordures
            canvas.DrawFilledRectangle(StyleManager.WindowBg, X, Y, Width, Height);
            canvas.DrawFilledRectangle(StyleManager.TitleBar, X, Y, Width, 30);

            // Titre
            canvas.DrawString(Title, PCScreenFont.Default, StyleManager.TextWhite, X + 10, Y + 8);

            // Boutons de contrôle
            canvas.DrawString("_", PCScreenFont.Default, StyleManager.TextWhite, X + Width - 45, Y + 5);
            canvas.DrawString("X", PCScreenFont.Default, StyleManager.CloseButton, X + Width - 20, Y + 8);
        }
    }
}