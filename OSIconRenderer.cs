using Cosmos.System.Graphics;
using System.Drawing;

namespace filesys.GUI
{
    public static class OSIconRenderer
    {
        // 💻 CONSOLE ICON
        public static void DrawConsole(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Black), x, y, 50, 40);
            canvas.DrawLine(new Pen(Color.Lime), x + 10, y + 10, x + 20, y + 20);
            canvas.DrawLine(new Pen(Color.Lime), x + 10, y + 20, x + 20, y + 10);
            canvas.DrawLine(new Pen(Color.Lime), x + 25, y + 25, x + 40, y + 25);
        }

        // 📁 FILE ICON
        public static void DrawFile(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.LightGray), x, y, 45, 50);
            canvas.DrawFilledRectangle(new Pen(Color.White), x + 5, y + 5, 35, 40);
            canvas.DrawLine(new Pen(Color.Gray), x + 10, y + 15, x + 35, y + 15);
            canvas.DrawLine(new Pen(Color.Gray), x + 10, y + 25, x + 35, y + 25);
        }

        // 📂 FOLDER ICON
        public static void DrawFolder(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Goldenrod), x, y + 10, 50, 35);
            canvas.DrawFilledRectangle(new Pen(Color.Khaki), x + 5, y, 25, 15);
        }

        // 📊 TASK MANAGER ICON
        public static void DrawTaskManager(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.DarkBlue), x, y, 50, 50);

            canvas.DrawLine(new Pen(Color.White), x + 10, y + 40, x + 10, y + 30);
            canvas.DrawLine(new Pen(Color.White), x + 20, y + 40, x + 20, y + 20);
            canvas.DrawLine(new Pen(Color.White), x + 30, y + 40, x + 30, y + 10);
            canvas.DrawLine(new Pen(Color.White), x + 40, y + 40, x + 40, y + 25);
        }
    }
}