using Cosmos.System;
using Cosmos.System.Graphics;
using filesys.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace filesys
{
    public static class CustomMouse
    {
        public static CursorStyle CurrentStyle = CursorStyle.Arrow;

        // Ajout des champs statiques pour X et Y
        public static int X { get; private set; }
        public static int Y { get; private set; }

        public static void Draw(Canvas canvas)
        {
            int x = (int)MouseManager.X;
            int y = (int)MouseManager.Y;

            // sécurité écran
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > ScreenManager.Width - 10) x = ScreenManager.Width - 10;
            if (y > ScreenManager.Height - 14) y = ScreenManager.Height - 14;

            var whitePen = new Pen(Color.White);

            switch (CurrentStyle)
            {
                case CursorStyle.Arrow:
                    DrawArrow(canvas, whitePen, x, y);
                    break;

                case CursorStyle.Cross:
                    DrawCross(canvas, whitePen, x, y);
                    break;

                case CursorStyle.Block:
                    DrawBlock(canvas, whitePen, x, y);
                    break;
            }
        }

        static void DrawArrow(Canvas canvas, Pen pen, int x, int y)
        {
            canvas.DrawFilledRectangle(pen, x, y, 10, 2);
            canvas.DrawFilledRectangle(pen, x, y, 2, 14);
        }

        static void DrawCross(Canvas canvas, Pen pen, int x, int y)
        {
            canvas.DrawFilledRectangle(pen, x - 5, y, 10, 1);
            canvas.DrawFilledRectangle(pen, x, y - 5, 1, 10);
        }

        static void DrawBlock(Canvas canvas, Pen pen, int x, int y)
        {
            canvas.DrawFilledRectangle(pen, x, y, 8, 8);
        }


    }
}
