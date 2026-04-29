using System.Drawing;
using Cosmos.System.Graphics;

namespace filesys.GUI
{
    public static class StyleManager
    {
   
        public static readonly Pen WindowBg = new Pen(Color.FromArgb(40, 40, 40));
        public static readonly Pen TitleBar = new Pen(Color.FromArgb(25, 25, 25));
        public static readonly Pen TextWhite = new Pen(Color.White);
        public static readonly Pen TextLime = new Pen(Color.Lime);
        public static readonly Pen CloseButton = new Pen(Color.Red);
        public static readonly Pen TaskbarBg = new Pen(Color.FromArgb(15, 15, 15));
        public static readonly Pen DarkGray = new Pen(Color.DarkGray);
        public static readonly Pen LightGray = new Pen(Color.LightGray);
        public static readonly Pen TextBlack = new Pen(Color.Black); // ajouté
        public static Color DesktopBackgroundColor = Color.Gray;
        // 🎨 Couleur de fond (PAS un Pen)


        public static Color GetColorFromIndex(int index)
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
        }
    }