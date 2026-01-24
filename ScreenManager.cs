using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filesys
{
    static class ScreenManager
    {
        public static int Width;
        public static int Height;
        public static Canvas Canvas;

        public static void Init(int width, int height)
        {
            Width = width;
            Height = height;

            Canvas = FullScreenCanvas.GetFullScreenCanvas(
                new Mode(width, height, ColorDepth.ColorDepth32)
            );

            // INITIALISATION SOURIS OBLIGATOIRE
            MouseManager.ScreenWidth = (uint)width;
            MouseManager.ScreenHeight = (uint)height;
        }
    }

}
