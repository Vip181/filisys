using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;
using System.IO;

namespace filesys.GUI
{
    public class ScreenSettingsWindow : BaseWindow
    {
        private Color[] colors;
        private string[] colorNames;

        private int selectedIndex = 0;
        private int hoverIndex = -1;

        private int scrollOffset = 0;

        private float scrollVelocity = 0f;
        private int lastMouseY = 0;

        private const int ItemHeight = 22;
        private const int VisibleItems = 12;

        private const string ConfigFile = @"0:\screen.cfg";

        private int maxScroll => Math.Max(0, colors.Length - VisibleItems);

        public ScreenSettingsWindow(int x, int y)
            : base("Screen settings", x, y, 520, 360)
        {
            LoadAllColors();
            LoadSavedColor();
        }

        // ================= COLORS =================
        private void LoadAllColors()
        {
            colors = new Color[]
            {
        Color.Black,
        Color.White,
        Color.Gray,
        Color.DarkGray,
        Color.LightGray,

        Color.Red,
        Color.Green,
        Color.Blue,
        Color.Yellow,
        Color.Orange,
        Color.Purple,
        Color.Pink,
        Color.Brown,
        Color.Cyan,
        Color.Magenta
            };

            colorNames = new string[]
            {
        "Black",
        "White",
        "Gray",
        "Dark Gray",
        "Light Gray",
        "Red",
        "Green",
        "Blue",
        "Yellow",
        "Orange",
        "Purple",
        "Pink",
        "Brown",
        "Cyan",
        "Magenta"
            };
        

        colorNames = new string[]
            {
                "Black","White","Gray","Dark Gray","Light Gray",
                "Red","Dark Red","Green","Dark Green","Blue","Dark Blue",
                "Yellow","Orange","Purple","Pink","Brown","Cyan","Magenta",
                "Orange RGB","Purple RGB","Sky Blue","Deep Pink",
                "Lime","Gold","Spring Green","Steel Blue"
            };
        }

        private void LoadSavedColor()
        {
            try
            {
                if (File.Exists(ConfigFile))
                {
                    int index = int.Parse(File.ReadAllText(ConfigFile));
                    if (index >= 0 && index < colors.Length)
                        selectedIndex = index;
                }

                StyleManager.DesktopBackgroundColor = colors[selectedIndex];
            }
            catch { }
        }

        // ================= UPDATE =================

        public override void Update()
        {
            base.Update();
            if (IsClosed || IsMinimized) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            int listX = X + 10;
            int listY = Y + 40;
            int listW = 260;
            int listH = VisibleItems * ItemHeight;

            // ================= HOVER =================
            hoverIndex = -1;

            if (mx >= listX && mx <= listX + listW &&
                my >= listY && my <= listY + listH)
            {
                hoverIndex = (my - listY) / ItemHeight + scrollOffset;

                if (hoverIndex >= colors.Length)
                    hoverIndex = -1;
            }

            // ================= CLICK SELECT =================
            if (click && hoverIndex != -1)
            {
                selectedIndex = hoverIndex;
                StyleManager.DesktopBackgroundColor = colors[selectedIndex];
            }

            // ================= SCROLL (COSMOS SAFE) =================
            int scrollDelta = my - lastMouseY;
            lastMouseY = my;

            if (mx >= listX && mx <= listX + listW &&
                my >= listY && my <= listY + listH)
            {
                if (scrollDelta != 0)
                    scrollVelocity += scrollDelta * 0.5f;
            }

            // inertia
            scrollVelocity *= 0.85f;

            scrollOffset += (int)scrollVelocity;

            if (scrollOffset < 0) scrollOffset = 0;
            if (scrollOffset > maxScroll) scrollOffset = maxScroll;

            if (Math.Abs(scrollVelocity) < 0.05f)
                scrollVelocity = 0;

            // ================= BUTTONS =================
            if (click)
            {
                if (ButtonHit(X + Width - 120, Y + 60)) Save();
                if (ButtonHit(X + Width - 120, Y + 100)) Reset();
                if (ButtonHit(X + Width - 120, Y + Height - 50)) IsClosed = true;
            }
        }

        // ================= DRAW =================

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if (IsClosed || IsMinimized) return;

            int listX = X + 10;
            int listY = Y + 40;
            int listW = 260;

            canvas.DrawFilledRectangle(
                new Pen(Color.DarkGray),
                listX, listY,
                listW, VisibleItems * ItemHeight
            );

            for (int i = 0; i < VisibleItems; i++)
            {
                int index = i + scrollOffset;
                if (index >= colors.Length) break;

                int iy = listY + i * ItemHeight;

                Color c = colors[index];

                // hover preview (brighten)
                if (index == hoverIndex)
                {
                    canvas.DrawFilledRectangle(
                        new Pen(Color.Gray),
                        listX, iy, listW, ItemHeight
                    );
                }

                // selected highlight
                if (index == selectedIndex)
                {
                    canvas.DrawFilledRectangle(
                        new Pen(Color.Gray),
                        listX, iy, listW, ItemHeight
                    );
                }

                // color box
                canvas.DrawFilledRectangle(
                    new Pen(colors[index]),
                    listX + 4, iy + 4, 14, 14
                );

                // text
                canvas.DrawString(
                    colorNames[index],
                    PCScreenFont.Default,
                    new Pen(Color.White),
                    listX + 24,
                    iy + 5
                );
            }

            DrawButton(canvas, "Save", X + Width - 120, Y + 60);
            DrawButton(canvas, "Reset", X + Width - 120, Y + 100);
            DrawButton(canvas, "Close", X + Width - 120, Y + Height - 50);
        }

        // ================= UI =================

        private void DrawButton(Canvas canvas, string text, int x, int y)
        {
            canvas.DrawFilledRectangle(
                new Pen(Color.Gray),
                x, y, 100, 28
            );

            canvas.DrawString(
                text,
                PCScreenFont.Default,
                new Pen(Color.White),
                x + 25,
                y + 7
            );
        }

        private bool ButtonHit(int x, int y)
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            return mx >= x && mx <= x + 100 &&
                   my >= y && my <= y + 28;
        }

        // ================= ACTIONS =================

        private void Save()
        {
            try
            {
                File.WriteAllText(ConfigFile, selectedIndex.ToString());
                StyleManager.DesktopBackgroundColor = colors[selectedIndex];
            }
            catch { }
        }

        private void Reset()
        {
            selectedIndex = 0;
            StyleManager.DesktopBackgroundColor = colors[0];
        }
    }
}