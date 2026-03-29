using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.System;
using System.Drawing;

namespace filesys.GUI
{
    public class StartMenu
    {
        private const int WIDTH = 420;
        private const int HEIGHT = 500;

        private bool isOpen = false;
        private int selected = -1;

        private string[] items =
        {
            "Menu",
            "Option",
            "Application",
            "Parametre",
            "Compte"
        };

        public void Toggle()
        {
            isOpen = !isOpen;
        }

        public void Update(bool click, bool lastClick)
        {
            if (!isOpen) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int menuTop = ScreenManager.Height - HEIGHT - 42;

            // Fermer si clic extérieur
            if (click && !lastClick)
            {
                bool inside =
                    mx >= 20 &&
                    mx <= 20 + WIDTH &&
                    my >= menuTop &&
                    my <= menuTop + HEIGHT;

                if (!inside)
                    isOpen = false;
            }

            // Hover
            selected = -1;
            for (int i = 0; i < items.Length; i++)
            {
                int y = menuTop + 40 + i * 60;

                if (mx >= 20 &&
                    mx <= 20 + WIDTH &&
                    my >= y &&
                    my <= y + 50)
                {
                    selected = i;
                }
            }

            HandlePowerButtons(click, lastClick, menuTop);
        }

        private void HandlePowerButtons(bool click, bool lastClick, int menuTop)
        {
            if (!click || lastClick) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int y = menuTop + HEIGHT - 60;

            if (mx >= 40 && mx <= 180 &&
                my >= y && my <= y + 35)
                Power.Reboot();

            if (mx >= 220 && mx <= 360 &&
                my >= y && my <= y + 35)
                Power.Shutdown();
        }

        public void Draw(Canvas canvas)
        {
            if (!isOpen) return;

            int menuTop = ScreenManager.Height - HEIGHT - 42;

            // Fond menu
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(240, 30, 30, 30)),
                20, menuTop, WIDTH, HEIGHT
            );

            for (int i = 0; i < items.Length; i++)
            {
                int y = menuTop + 40 + i * 60;

                Color bg = (i == selected)
                    ? Color.FromArgb(0, 140, 255)
                    : Color.FromArgb(60, 60, 60);

                canvas.DrawFilledRectangle(
                    new Pen(bg),
                    20, y, WIDTH, 50
                );

                canvas.DrawString(
                    items[i],
                    PCScreenFont.Default,
                    GraphicsManager.WhitePen,
                    40, y + 18
                );
            }

            DrawPowerButtons(canvas, menuTop);
        }

        private void DrawPowerButtons(Canvas canvas, int menuTop)
        {
            int y = menuTop + HEIGHT - 60;

            // Restart
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(80, 80, 80)),
                40, y, 140, 35
            );

            canvas.DrawString(
                "Restart",
                PCScreenFont.Default,
                GraphicsManager.WhitePen,
                70, y + 10
            );

            // Shutdown
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(150, 40, 40)),
                220, y, 140, 35
            );

            canvas.DrawString(
                "Shutdown",
                PCScreenFont.Default,
                GraphicsManager.WhitePen,
                245, y + 10
            );
        }
    }
}