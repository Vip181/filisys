using Cosmos.System.Graphics;
using System.Drawing;

namespace filesys.GUI
{
    public class UiBitmapCache
    {
        private byte[] data;
        private int width;
        private int height;
        private Bitmap bitmap;

        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        public UiBitmapCache(int w, int h)
        {
            width = w;
            height = h;
            data = CreateBitmapBytes(width, height);
            bitmap = new Bitmap(data);
        }

        public void Clear(Color color)
        {
            FillRect(0, 0, width, height, color);
            bitmap = new Bitmap(data);
        }

        public void FillRect(int x, int y, int w, int h, Color color)
        {
            for (int yy = y; yy < y + h; yy++)
            {
                for (int xx = x; xx < x + w; xx++)
                {
                    SetPixel(xx, yy, color);
                }
            }
        }

        public void Border(int x, int y, int w, int h, Color color)
        {
            FillRect(x, y, w, 1, color);
            FillRect(x, y + h - 1, w, 1, color);
            FillRect(x, y, 1, h, color);
            FillRect(x + w - 1, y, 1, h, color);
        }

        public void Apply()
        {
            bitmap = new Bitmap(data);
        }

        private void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;

            int realY = height - 1 - y;
            int index = 54 + ((realY * width + x) * 4);

            data[index + 0] = color.B;
            data[index + 1] = color.G;
            data[index + 2] = color.R;
            data[index + 3] = 255;
        }

        private static byte[] CreateBitmapBytes(int width, int height)
        {
            int headerSize = 54;
            int imageSize = width * height * 4;
            int fileSize = headerSize + imageSize;

            byte[] bmp = new byte[fileSize];

            bmp[0] = (byte)'B';
            bmp[1] = (byte)'M';

            WriteInt(bmp, 2, fileSize);
            WriteInt(bmp, 10, headerSize);
            WriteInt(bmp, 14, 40);
            WriteInt(bmp, 18, width);
            WriteInt(bmp, 22, height);

            bmp[26] = 1;
            bmp[28] = 32;

            WriteInt(bmp, 34, imageSize);

            return bmp;
        }

        private static void WriteInt(byte[] data, int index, int value)
        {
            data[index + 0] = (byte)value;
            data[index + 1] = (byte)(value >> 8);
            data[index + 2] = (byte)(value >> 16);
            data[index + 3] = (byte)(value >> 24);
        }
    }
}