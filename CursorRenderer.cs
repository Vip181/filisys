using Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;

namespace filesys.GUI
{
    public static class CursorRenderer
    {
        private static Bitmap cursorBitmap;
        private static bool ready = false;

        private const int Scale = 1;

        private static readonly string[] Cursor =
        {
            "100000000000",
            "120000000000",
            "122000000000",
            "122200000000",
            "122220000000",
            "122222000000",
            "122222200000",
            "122222220000",
            "122222222000",
            "122222222200",
            "122222000000",
            "122012200000",
            "120001220000",
            "100000122000",
            "000000012200",
            "000000001100"
        };

        public static void Init()
        {
            if (ready)
                return;

            int width = Cursor[0].Length * Scale;
            int height = Cursor.Length * Scale;

            byte[] bmp = CreateBitmapBytes(width, height);

            for (int y = 0; y < Cursor.Length; y++)
            {
                for (int x = 0; x < Cursor[y].Length; x++)
                {
                    char p = Cursor[y][x];

                    if (p == '0')
                        continue;

                    Color color = p == '1' ? Color.Black : Color.White;

                    for (int sy = 0; sy < Scale; sy++)
                    {
                        for (int sx = 0; sx < Scale; sx++)
                        {
                            SetBmpPixel(
                                bmp,
                                width,
                                height,
                                x * Scale + sx,
                                y * Scale + sy,
                                color);
                        }
                    }
                }
            }

            cursorBitmap = new Bitmap(bmp);
            ready = true;
        }

        public static void Draw(Canvas canvas)
        {
            if (!ready)
                Init();

            int x = (int)MouseManager.X;
            int y = (int)MouseManager.Y;

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            if (x > ScreenManager.Width - cursorBitmap.Width)
                x = (int)(ScreenManager.Width - cursorBitmap.Width);

            if (y > ScreenManager.Height - cursorBitmap.Height)
                y = (int)(ScreenManager.Height - cursorBitmap.Height);

            // Si ton Cosmos accepte DrawImageAlpha, utilise ça :
            canvas.DrawImageAlpha(cursorBitmap, x, y);

            // Si DrawImageAlpha n'existe pas chez toi, remplace par :
            // canvas.DrawImage(cursorBitmap, x, y);
        }

        private static byte[] CreateBitmapBytes(int width, int height)
        {
            int headerSize = 54;
            int imageSize = width * height * 4;
            int fileSize = headerSize + imageSize;

            byte[] data = new byte[fileSize];

            data[0] = (byte)'B';
            data[1] = (byte)'M';

            WriteInt(data, 2, fileSize);
            WriteInt(data, 10, headerSize);
            WriteInt(data, 14, 40);
            WriteInt(data, 18, width);
            WriteInt(data, 22, height);

            data[26] = 1;
            data[28] = 32;

            WriteInt(data, 34, imageSize);

            return data;
        }

        private static void SetBmpPixel(byte[] bmp, int width, int height, int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;

            int headerSize = 54;

            int realY = height - 1 - y;
            int index = headerSize + ((realY * width + x) * 4);

            bmp[index + 0] = color.B;
            bmp[index + 1] = color.G;
            bmp[index + 2] = color.R;
            bmp[index + 3] = 255;
        }

        private static void WriteInt(byte[] data, int index, int value)
        {
            data[index + 0] = (byte)(value);
            data[index + 1] = (byte)(value >> 8);
            data[index + 2] = (byte)(value >> 16);
            data[index + 3] = (byte)(value >> 24);
        }
    }
}