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
        // ================= CONFIG =================
        private const string ConfigFile = @"0:\screen.cfg";
        private const int DefaultColorIndex = 0;
        private const int DefaultWidth = 1024;   // ⚠ SAFE
        private const int DefaultHeight = 768;   // ⚠ SAFE

        // ================= COLORS =================
        private Color[] colors;
        private string[] colorNames;
        private int selectedColorIndex = 0;
        private int hoverColorIndex = -1;
        private int colorScrollOffset = 0;

        // ================= RESOLUTIONS =================
        private (int W, int H)[] resolutions;
        private string[] resolutionNames;
        private int selectedResolutionIndex = 0;
        private int hoverResolutionIndex = -1;
        private int resolutionScrollOffset = 0;

        // ================= UI =================
        private const int ItemHeight = 22;
        private const int VisibleItems = 10;

        public ScreenSettingsWindow(int x, int y)
            : base("Screen settings", x, y, 560, 380)
        {
            LoadColors();
            LoadResolutions();
            EnsureConfigFile();
            LoadConfig();
        }

        // =====================================================
        // LOAD
        // =====================================================
        private void LoadColors()
        {
            colors = new Color[]
            {
                Color.Black, Color.White, Color.Gray, Color.DarkGray, Color.LightGray,
                Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange,
                Color.Purple, Color.Pink, Color.Brown, Color.Cyan, Color.Magenta
            };

            colorNames = new string[]
            {
                "Black","White","Gray","Dark Gray","Light Gray",
                "Red","Green","Blue","Yellow","Orange",
                "Purple","Pink","Brown","Cyan","Magenta"
            };
        }

        private void LoadResolutions()
        {
            // ⚠️ Résolutions SAFE sous VMware
            resolutions = new (int, int)[]
            {
                (800, 600),
                (1024, 768),
                (1280, 720),
                (1366, 768)
            };

            resolutionNames = new string[]
            {
                "800 x 600",
                "1024 x 768",
                "1280 x 720",
                "1366 x 768"
            };
        }

        private void EnsureConfigFile()
        {
            if (File.Exists(ConfigFile))
                return;

            try
            {
                File.WriteAllLines(ConfigFile, new string[]
                {
                    DefaultColorIndex.ToString(),
                    $"{DefaultWidth},{DefaultHeight}"
                });
            }
            catch { }
        }

        private void LoadConfig()
        {
            try
            {
                string[] lines = File.ReadAllLines(ConfigFile);

                if (lines.Length >= 1)
                    selectedColorIndex = int.Parse(lines[0]);

                if (lines.Length >= 2)
                {
                    string[] res = lines[1].Split(',');
                    int w = int.Parse(res[0]);
                    int h = int.Parse(res[1]);

                    for (int i = 0; i < resolutions.Length; i++)
                    {
                        if (resolutions[i].W == w && resolutions[i].H == h)
                        {
                            selectedResolutionIndex = i;
                            break;
                        }
                    }
                }

                StyleManager.DesktopBackgroundColor = colors[selectedColorIndex];
            }
            catch
            {
                selectedColorIndex = DefaultColorIndex;
                selectedResolutionIndex = 0;
            }
        }

        // =====================================================
        // UPDATE
        // =====================================================
        public override void Update()
        {
            base.Update();
            if (IsClosed || IsMinimized) return;

            bool click = MouseManager.MouseState == MouseState.Left;

            // ---- COLOR LIST ----
            UpdateListBox(
                X + 10, Y + 40,
                ref hoverColorIndex,
                ref selectedColorIndex,
                ref colorScrollOffset,
                colors.Length,
                click
            );

            if (click && hoverColorIndex != -1)
                StyleManager.DesktopBackgroundColor = colors[selectedColorIndex];

            // ---- RESOLUTION LIST ----
            UpdateListBox(
                X + 300, Y + 40,
                ref hoverResolutionIndex,
                ref selectedResolutionIndex,
                ref resolutionScrollOffset,
                resolutions.Length,
                click
            );

            // ---- OK BUTTON ----
            if (click && ButtonHit(X + Width / 2 - 50, Y + Height - 45))
            {
                Save();
                IsClosed = true;
            }
        }

        private void UpdateListBox(
            int x, int y,
            ref int hover,
            ref int selected,
            ref int scroll,
            int count,
            bool click)
        {
            hover = -1;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            if (mx >= x && mx <= x + 240 &&
                my >= y && my <= y + VisibleItems * ItemHeight)
            {
                hover = (my - y) / ItemHeight + scroll;
                if (hover >= count) hover = -1;
            }

            if (click && hover != -1)
                selected = hover;
        }

        // =====================================================
        // DRAW
        // =====================================================
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if (IsClosed || IsMinimized) return;

            DrawListBox(
                canvas,
                X + 10, Y + 40,
                colors.Length,
                colorScrollOffset,
                hoverColorIndex,
                selectedColorIndex,
                i => colorNames[i],
                i => colors[i]
            );

            DrawListBox(
                canvas,
                X + 300, Y + 40,
                resolutions.Length,
                resolutionScrollOffset,
                hoverResolutionIndex,
                selectedResolutionIndex,
                i => resolutionNames[i],
                null
            );

            DrawButton(canvas, "OK", X + Width / 2 - 50, Y + Height - 45);
        }

        private void DrawListBox(
            Canvas canvas,
            int x, int y,
            int count,
            int scroll,
            int hover,
            int selected,
            Func<int, string> text,
            Func<int, Color> colorBox)
        {
            canvas.DrawFilledRectangle(
                new Pen(Color.DarkGray),
                x, y, 240, VisibleItems * ItemHeight
            );

            for (int i = 0; i < VisibleItems; i++)
            {
                int index = i + scroll;
                if (index >= count) break;

                int iy = y + i * ItemHeight;

                if (index == hover || index == selected)
                    canvas.DrawFilledRectangle(
                        new Pen(Color.Gray),
                        x, iy, 240, ItemHeight
                    );

                if (colorBox != null)
                    canvas.DrawFilledRectangle(
                        new Pen(colorBox(index)),
                        x + 4, iy + 4, 14, 14
                    );

                canvas.DrawString(
                    text(index),
                    PCScreenFont.Default,
                    new Pen(Color.White),
                    x + (colorBox != null ? 24 : 10),
                    iy + 5
                );
            }
        }

        private void DrawButton(Canvas canvas, string text, int x, int y)
        {
            canvas.DrawFilledRectangle(
                new Pen(Color.Gray),
                x, y, 100, 30
            );

            canvas.DrawString(
                text,
                PCScreenFont.Default,
                new Pen(Color.White),
                x + 35,
                y + 8
            );
        }

        private bool ButtonHit(int x, int y)
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            return mx >= x && mx <= x + 100 &&
                   my >= y && my <= y + 30;
        }

        // =====================================================
        // SAVE
        // =====================================================
        private void Save()
        {
            try
            {
                var res = resolutions[selectedResolutionIndex];

                File.WriteAllLines(ConfigFile, new string[]
                {
                    selectedColorIndex.ToString(),
                    $"{res.W},{res.H}"
                });
            }
            catch { }
        }
    }
}