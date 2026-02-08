using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;
using filesys.System;

namespace filesys.GUI
{
    public class Window
    {
        // Position & taille
        public int X, Y, Width, Height;
        public string Title;

        // Mémoire dédiée à la fenêtre
        public AppMemoryContext Memory;

        // Drag
        protected bool dragging;
        protected int dragOffsetX, dragOffsetY;

        public Window(string title, int x, int y, int w, int h)
        {
            Title = title;
            X = x;
            Y = y;
            Width = w;
            Height = h;

            // 🔥 Contexte mémoire propre
            Memory = OsMemoryManager.CreateApp(title);
        }

        public virtual void Update()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            bool onTitleBar =
                mx >= X && mx <= X + Width &&
                my >= Y && my <= Y + 24;

            if (MouseManager.MouseState == MouseState.Left && onTitleBar && !dragging)
            {
                dragging = true;
                dragOffsetX = mx - X;
                dragOffsetY = my - Y;
            }

            if (MouseManager.MouseState == MouseState.None)
                dragging = false;

            if (dragging)
            {
                X = mx - dragOffsetX;
                Y = my - dragOffsetY;
            }

            KeepInScreen();
        }

        protected void KeepInScreen()
        {
            if (X < 0) X = 0;
            if (Y < 0) Y = 0;

            if (X + Width > ScreenManager.Width)
                X = ScreenManager.Width - Width;

            if (Y + Height > ScreenManager.Height)
                Y = ScreenManager.Height - Height;
        }

        public virtual void Draw(Canvas canvas)
        {
            if (canvas == null) return;

            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(50, 50, 50)),
                X, Y, Width, Height);

            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(70, 70, 70)),
                X, Y, Width, 24);

            canvas.DrawString(
                Title,
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 5,
                Y + 5);
        }
        public bool IsInTitleBar(int mx, int my)
        {
            return mx >= X && mx <= X + Width &&
                   my >= Y && my <= Y + 24;
        }
        // ❌ Fermeture propre
        public virtual void Close()
        {
            OsMemoryManager.DestroyApp(Memory);
        }
    }
}
