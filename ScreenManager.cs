using System;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace filesys
{
    public static class ScreenManager
    {
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        // Canvas pleinement qualifié pour lever toute ambiguïté
        public static Cosmos.System.Graphics.Canvas Canvas { get; private set; }

        public static void Init(int width, int height)
        {
            Width = width;
            Height = height;

            // Crée le Canvas plein écran (Mode et ColorDepth sont dans Cosmos.System.Graphics)
            Canvas = FullScreenCanvas.GetFullScreenCanvas(
                new Mode(width, height, ColorDepth.ColorDepth32)
            );

            // Initialiser la souris pour correspondre à la résolution
            MouseManager.ScreenWidth = (uint)width;
            MouseManager.ScreenHeight = (uint)height;
        }

        public static void Destroy()
        {
            // Desserrez les références pour permettre au GC du runtime Cosmos de libérer
            Canvas = null;
        }
    }
}