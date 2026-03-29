using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.System;
using System.Drawing;

namespace filesys.GUI
{
    public class Taskbar
    {
        private const int HEIGHT = 48;
        private const int START_SIZE = 40;

        private const int MENU_WIDTH = 650;
        private const int MENU_HEIGHT = 500;
        private const int CATEGORY_WIDTH = 220;

        private bool mouseWasPressed = false;
        private bool hoverStart = false;
        private bool startMenuOpen = false;

        private int selectedCategory = -1;

        private string[] categories =
        {
            "Administration",
            "Bureautique",
            "Configuration",
            "Développement",
            "Graphisme",
            "Internet",
            "Jeux",
            "Multimédia",
            "Système"
        };

        // =============================
        // UPDATE
        // =============================
        public void Update()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool pressed = MouseManager.MouseState == MouseState.Left;

            hoverStart =
                my >= ScreenManager.Height - HEIGHT &&
                mx >= 10 && mx <= 10 + START_SIZE;

            if (pressed && !mouseWasPressed)
                HandleClick(mx, my);

            mouseWasPressed = pressed;

            if (startMenuOpen)
                UpdateHover(mx, my);
        }

        // =============================
        // CLICK
        // =============================
        private void HandleClick(int mx, int my)
        {
            int screenH = ScreenManager.Height;
            int menuTop = screenH - HEIGHT - MENU_HEIGHT;

            // Bouton start rond
            if (my >= screenH - HEIGHT &&
                mx >= 10 && mx <= 10 + START_SIZE)
            {
                startMenuOpen = !startMenuOpen;
               ;
                return;
            }

            if (!startMenuOpen)
                return;

            // Fermer si clic extérieur
            bool inside =
                mx >= 10 &&
                mx <= 10 + MENU_WIDTH &&
                my >= menuTop &&
                my <= menuTop + MENU_HEIGHT;

            if (!inside)
            {
                startMenuOpen = false;
               
                return;
            }

            HandleCategoryClick(mx, my, menuTop);
            HandlePowerClick(mx, my, menuTop);
        }

        // =============================
        // CATEGORIES
        // =============================
        private void HandleCategoryClick(int mx, int my, int menuTop)
        {
            for (int i = 0; i < categories.Length; i++)
            {
                int y = menuTop + 60 + i * 36;

                if (mx >= 20 &&
                    mx <= 20 + CATEGORY_WIDTH &&
                    my >= y &&
                    my <= y + 32)
                {
                    selectedCategory = i;
                }
            }

            if (selectedCategory == 4) // Graphisme
            {
                int startX = 20 + CATEGORY_WIDTH + 30;
                int startY = menuTop + 80;

                if (mx >= startX && mx <= startX + 220 &&
                    my >= startY && my <= startY + 35)
                {
                  
                    startMenuOpen = false;
                }

                if (mx >= startX && mx <= startX + 220 &&
                    my >= startY + 45 && my <= startY + 80)
                {
                  
                    startMenuOpen = false;
                }
            }

          
        }

        private void HandlePowerClick(int mx, int my, int menuTop)
        {
            int y = menuTop + MENU_HEIGHT - 60;

            if (mx >= 30 && mx <= 150 &&
                my >= y && my <= y + 35)
                Power.Reboot();

            if (mx >= 170 && mx <= 290 &&
                my >= y && my <= y + 35)
                Power.Shutdown();
        }

        private void UpdateHover(int mx, int my)
        {
            int menuTop = ScreenManager.Height - HEIGHT - MENU_HEIGHT;

            selectedCategory = -1;

            for (int i = 0; i < categories.Length; i++)
            {
                int y = menuTop + 60 + i * 36;

                if (mx >= 20 &&
                    mx <= 20 + CATEGORY_WIDTH &&
                    my >= y &&
                    my <= y + 32)
                {
                    selectedCategory = i;
                }
            }

           
        }

        // =============================
        // DRAW
        // =============================
        public void Draw(Canvas canvas)
        {
            DrawTaskbar(canvas);

            if (startMenuOpen)
                DrawMenu(canvas);
        }

        private void DrawTaskbar(Canvas canvas)
        {
            int w = ScreenManager.Width;
            int h = ScreenManager.Height;

            // Fond barre moderne
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(28, 28, 28)),
                0, h - HEIGHT, w, HEIGHT
            );

            // Bouton start rond
            Color startColor = hoverStart
                ? Color.FromArgb(90, 90, 90)
                : Color.FromArgb(60, 60, 60);

            canvas.DrawFilledRectangle(
                new Pen(startColor),
                10,
                h - HEIGHT + 4,
                START_SIZE,
                START_SIZE
            );

            canvas.DrawString(
                "≡",
                PCScreenFont.Default,
                GraphicsManager.WhitePen,
                25,
                h - HEIGHT + 15
            );
        }

        private void DrawMenu(Canvas canvas)
        {
            int screenH = ScreenManager.Height;
            int menuTop = screenH - HEIGHT - MENU_HEIGHT;

            // Fond moderne large
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(230, 32, 32, 32)),
                10,
                menuTop,
                MENU_WIDTH,
                MENU_HEIGHT
            );

            DrawCategories(canvas, menuTop);
            DrawSubMenu(canvas, menuTop);
            DrawPowerButtons(canvas, menuTop);
        }

        private void DrawCategories(Canvas canvas, int menuTop)
        {
            for (int i = 0; i < categories.Length; i++)
            {
                int y = menuTop + 60 + i * 36;

                Color bg = (i == selectedCategory)
                    ? Color.FromArgb(0, 120, 215)
                    : Color.FromArgb(45, 45, 45);

                canvas.DrawFilledRectangle(
                    new Pen(bg),
                    20,
                    y,
                    CATEGORY_WIDTH,
                    32
                );

                canvas.DrawString(
                    categories[i],
                    PCScreenFont.Default,
                    GraphicsManager.WhitePen,
                    35,
                    y + 8
                );
            }
        }

        private void DrawSubMenu(Canvas canvas, int menuTop)
        {
            if (selectedCategory != 4)
                return;

            int startX = 20 + CATEGORY_WIDTH + 30;
            int startY = menuTop + 80;

            DrawAppItem(canvas, startX, startY, "Console");
            DrawAppItem(canvas, startX, startY + 45, "About");
        }

        private void DrawAppItem(Canvas canvas, int x, int y, string name)
        {
            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(55, 55, 55)),
                x, y, 220, 35
            );

            canvas.DrawString(
                name,
                PCScreenFont.Default,
                GraphicsManager.WhitePen,
                x + 12,
                y + 10
            );
        }

        private void DrawPowerButtons(Canvas canvas, int menuTop)
        {
            int y = menuTop + MENU_HEIGHT - 60;

            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(70, 70, 70)),
                30, y, 120, 35
            );

            canvas.DrawString(
                "Restart",
                PCScreenFont.Default,
                GraphicsManager.WhitePen,
                55, y + 10
            );

            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(150, 40, 40)),
                170, y, 120, 35
            );

            canvas.DrawString(
                "Shutdown",
                PCScreenFont.Default,
                GraphicsManager.WhitePen,
                190, y + 10
            );
        }
    }
}