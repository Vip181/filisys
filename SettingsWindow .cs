using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace filesys.GUI
{
    public class SettingsWindow : BaseWindow
    {
        private int colorIndex = 0;
        private Color[] colors =
        {
            Color.White,
            Color.Lime,
            Color.Cyan,
            Color.Yellow,
            Color.Red
        };

        public SettingsWindow(int x, int y)
            : base("Display Settings", x, y, 260, 180)
        {
        }

        public override void Update()
        {
            base.Update();
            if (IsMinimized) return;

            if (KeyboardManager.TryReadKey(out var key))
            {
                // Changer police
                if (key.Key == ConsoleKeyEx.F1)
                {
                    UISettings.CurrentFont =
                        UISettings.CurrentFont == PCScreenFont.Default
                        ? PCScreenFont.Default // Remplacer Tiny par Default
                        : PCScreenFont.Default;
                }

                // Changer couleur
                if (key.Key == ConsoleKeyEx.F2)
                {
                    colorIndex = (colorIndex + 1) % colors.Length;
                    UISettings.TextColor = colors[colorIndex];
                }

                // Sauvegarder
                if (key.Key == ConsoleKeyEx.F5)
                {
                    UISettings.Save();
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized) return;

            base.Draw(canvas);

            int y = Y + 40;

            canvas.DrawString(
                "F1 : Change Font",
                UISettings.CurrentFont,
                new Pen(UISettings.TextColor),
                X + 10,
                y
            );

            y += 20;

            canvas.DrawString(
                "F2 : Change Color",
                UISettings.CurrentFont,
                new Pen(UISettings.TextColor),
                X + 10,
                y
            );

            y += 20;

            canvas.DrawString(
                "F5 : Save Settings",
                UISettings.CurrentFont,
                new Pen(Color.Yellow),
                X + 10,
                y
            );

            y += 30;

            canvas.DrawString(
                "Preview Text",
                UISettings.CurrentFont,
                new Pen(UISettings.TextColor),
                X + 10,
                y
            );
        }
    }
}