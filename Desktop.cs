using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filesys
{
    static class Desktop
    {
        public static void Draw(Canvas canvas)
        {
            // Correction : utiliser Pen au lieu de Color
            var backgroundPen = new Pen(Color.FromArgb(30, 30, 30));
            var barPen = new Pen(Color.FromArgb(45, 45, 45));

            canvas.DrawFilledRectangle(
                backgroundPen,
                0,
                0,
                ScreenManager.Width,
                ScreenManager.Height
            );

            // barre du bas adaptative
            canvas.DrawFilledRectangle(
                barPen,
                0,
                ScreenManager.Height - 28,
                ScreenManager.Width,
                28
            );
        }

        public static bool ClickConsole()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            return MouseManager.MouseState == MouseState.Left &&
                   mx >= 50 && mx <= 98 &&
                   my >= 50 && my <= 98;
        }
    }
}
