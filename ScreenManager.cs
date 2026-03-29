using Cosmos.System;
using Cosmos.System.Graphics;
using filesys.System;
using System;

namespace filesys
{
    public static class ScreenManager
    {
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static Canvas Canvas { get; private set; }

        private static AppMemoryContext memContext;

        public static void Init(int width, int height)
        {
            Width = width;
            Height = height;

            // Crée un contexte mémoire pour le ScreenManager (pour d'autres allocations si besoin)
            memContext = OsMemoryManager.CreateApp("ScreenManager");

            // Crée le Canvas concret FullScreenCanvas
            Canvas = FullScreenCanvas.GetFullScreenCanvas(
                new Mode(width, height, ColorDepth.ColorDepth32)
            );

            // Pas besoin de track le Canvas lui-même
           //memContext.Add((IntPtr)Canvas);

            // Initialisation souris
            MouseManager.ScreenWidth = (uint)width;
            MouseManager.ScreenHeight = (uint)height;
        }

        public static void Destroy()
        {
            if (memContext != null)
            {
                OsMemoryManager.DestroyApp(memContext);
                memContext = null;
                Canvas = null;
            }
        }
    }
}
