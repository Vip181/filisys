using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;

namespace filesys.GUI
{
    public class Button
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public string Text;

        public bool IsHovered;
        public bool IsPressed;

        public Action OnClick;

        public Button(int x, int y, int width, int height, string text, Action onClick)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Text = text;
            OnClick = onClick;
        }

        public void Update()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            IsHovered =
                mx >= X && mx <= X + Width &&
                my >= Y && my <= Y + Height;

            if (IsHovered && MouseManager.MouseState == MouseState.Left)
            {
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnClick?.Invoke();
                }
            }
            else
            {
                IsPressed = false;
            }
        }

        public void Draw(Canvas canvas)
        {
            Pen bgPen = new Pen(IsHovered ? Color.Gray : Color.DarkGray);
            Pen border = new Pen(Color.White);

            canvas.DrawFilledRectangle(bgPen, X, Y, Width, Height);
            canvas.DrawRectangle(border, X, Y, Width, Height);

            // Texte (simple, centré approximativement)
            canvas.DrawString(
                Text,
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 6,
                Y + (Height / 2) - 8
            );
        }
    }
}
