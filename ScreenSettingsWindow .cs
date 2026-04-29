using System;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;
using System.IO;
namespace filesys.GUI
{
    public class ScreenSettingsWindow : BaseWindow
    {
        private Color previewColor = Color.Gray;
        private Color savedColor = Color.Gray;
        private const string ConfigFile = @"0:\screen.cfg";
        private Color GetColorFromIndex(int index)
        {
            switch (index)
            {
                case 0: return Color.Red;
                case 1: return Color.Blue;
                case 2: return Color.Beige;
                case 3: return Color.Green;
                case 4: return Color.DarkGray;
                case 5: return Color.LightGray;
                default: return Color.Gray;
            }
        }
        private string[] items =
        {
            "Red screen",
            "Bleu screen",
            "Beige screen",
            "Green screen",
            "Dark screen",
            "Light screen",
        };

        private int selectedIndex = 0;

        public ScreenSettingsWindow(int x, int y)
            : base("Screen settings", x, y, 520, 360)
        {
        }

        public override void Update()
        {
            base.Update();
            if (IsClosed || IsMinimized) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            // --- Liste (clic sélection)
            int listX = X + 10;
            int listY = Y + 40;
            int itemH = 26;

            for (int i = 0; i < items.Length; i++)
            {
                int iy = listY + i * itemH;
                if (click &&
                    mx >= listX && mx <= listX + 180 &&
                    my >= iy && my <= iy + itemH)
                {
                    selectedIndex = i;
                }
            }

            // --- Boutons à droite
            if (click)
            {
                if (ButtonHit("test", X + Width - 120, Y + 60)) Test();
                if (ButtonHit("save", X + Width - 120, Y + 100)) Save();
                if (ButtonHit("reset", X + Width - 120, Y + 140)) Reset();
                if (ButtonHit("Ok", X + Width - 120, Y + Height - 50)) Ok();
            }
        }

        private bool ButtonHit(string txt, int bx, int by)
        {
            int bw = 100;
            int bh = 28;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            return mx >= bx && mx <= bx + bw &&
                   my >= by && my <= by + bh;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if (IsClosed || IsMinimized) return;

            // --- Panneau gauche (liste)
            int listX = X + 10;
            int listY = Y + 40;
            int listW = 200;
            int listH = Height - 60;

            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(255, 240, 200)),
                listX, listY, listW, listH);

            // Items
            int itemH = 26;
            for (int i = 0; i < items.Length; i++)
            {
                int iy = listY + i * itemH;

                if (i == selectedIndex)
                {
                    canvas.DrawFilledRectangle(
                        new Pen(Color.Gray),
                        listX + 5, iy + 4, listW - 10, itemH - 6
                    );
                }

                canvas.DrawString(
                    items[i],
                    PCScreenFont.Default,
                    new Pen(Color.White),
                    listX + 10,
                    iy + 8
                );
            }

            // --- Scrollbar (visuelle)
            canvas.DrawFilledRectangle(
                new Pen(Color.LightGray),
                listX + listW + 2,
                listY,
                12,
                listH
            );

            canvas.DrawFilledRectangle(
                new Pen(Color.Orange),
                listX + listW + 2,
                listY + 30,
                12,
                60
            );

            // --- Boutons à droite
            DrawButton(canvas, "test", X + Width - 120, Y + 60);
            DrawButton(canvas, "save", X + Width - 120, Y + 100);
            DrawButton(canvas, "reset", X + Width - 120, Y + 140);
            DrawButton(canvas, "Ok", X + Width - 120, Y + Height - 50);
        }

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
                x + 30,
                y + 7
            );
        }

        // --- Actions ---
        private void Test()
        {
            previewColor = GetColorFromIndex(selectedIndex);

            
            StyleManager.DesktopBackgroundColor = (previewColor);
        }

        private void Save()
        {
            try
            {
                savedColor = GetColorFromIndex(selectedIndex);

                // Sauvegarde simple (nom du thème)
               File.WriteAllText(ConfigFile, selectedIndex.ToString());

                // Appliquer définitivement
                StyleManager.DesktopBackgroundColor = savedColor;
            }
            catch
            {
                // Cosmos : ignorer silencieusement
            }
        }

        private void Reset()
        {
            selectedIndex = 0;
        }

        private void Ok()
        {
            IsClosed = true;
        }
    }
}