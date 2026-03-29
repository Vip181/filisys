using Cosmos.System.Graphics;
using System.Drawing;

namespace filesys.System
{
    public static class PixelEngine
    {
        private static int width;
        private static int height;

        private static Color[] backBuffer;
        private static Canvas canvas;
        private static Pen pen = new Pen(Color.Black);

        public static void Init(Canvas canv, int w, int h)
        {
            canvas = canv;
            width = w;
            height = h;

            backBuffer = new Color[w * h];
        }

        public static void Clear(Color c)
        {
            for (int i = 0; i < backBuffer.Length; i++)
                backBuffer[i] = c;
        }

        public static void SetPixel(int x, int y, Color c)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;

            int index = y * width + x;
            backBuffer[index] = c;
        }

        public static void RenderBackground()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;

                    pen.Color = backBuffer[index];
                    canvas.DrawPoint(pen, x, y);
                }
            }
        }
    }
}
