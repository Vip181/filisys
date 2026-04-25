using Cosmos.System.Graphics;
using System.Drawing;

namespace filesys.GUI
{
    public static class UIIcons
    {
        // 💻 CONSOLE ICON (style terminal)
        public static void Console(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Black), x, y, 40, 30);

            // prompt >
            canvas.DrawLine(new Pen(Color.Lime), x + 8, y + 10, x + 15, y + 17);
            canvas.DrawLine(new Pen(Color.Lime), x + 8, y + 17, x + 15, y + 10);

            // underscore
            canvas.DrawLine(new Pen(Color.Lime), x + 18, y + 20, x + 30, y + 20);
        }

        // 📁 FILES ICON (document)
        public static void File(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.White), x, y, 40, 45);
            canvas.DrawFilledRectangle(new Pen(Color.LightGray), x + 5, y + 5, 30, 35);

            canvas.DrawLine(new Pen(Color.Gray), x + 10, y + 15, x + 30, y + 15);
            canvas.DrawLine(new Pen(Color.Gray), x + 10, y + 22, x + 30, y + 22);
        }

        // 📂 FOLDER ICON
        public static void Folder(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Goldenrod), x, y + 10, 42, 30);
            canvas.DrawFilledRectangle(new Pen(Color.Khaki), x + 5, y, 20, 15);
        }

        // 📊 TASK MANAGER ICON
        public static void TaskManager(Canvas canvas, int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.SteelBlue), x, y, 42, 42);

            canvas.DrawLine(new Pen(Color.White), x + 10, y + 30, x + 10, y + 20);
            canvas.DrawLine(new Pen(Color.White), x + 20, y + 30, x + 20, y + 15);
            canvas.DrawLine(new Pen(Color.White), x + 30, y + 30, x + 30, y + 10);
        }
    }
}