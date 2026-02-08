using Cosmos.System;
using Cosmos.System.Graphics;
using filesys.GUI;
namespace filesys
{
    static class WindowManager
    {
        static Window[] windows = new Window[10];
        static int count = 0;

        static Window focused;

        public static void Add(Window win)
        {
            if (count < windows.Length)
                windows[count++] = win;
        }

        public static void Update()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            // gestion du focus (clic sur barre de titre)
            if (MouseManager.MouseState == MouseState.Left)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    if (windows[i].IsInTitleBar(mx, my))
                    {
                        focused = windows[i];
                        BringToFront(i);
                        break;
                    }
                }
            }

            // update toutes les fenêtres
            for (int i = 0; i < count; i++)
            {
                windows[i].Update();
            }
        }

        public static void Draw(Canvas canvas)
        {
            for (int i = 0; i < count; i++)
            {
                windows[i].Draw(canvas);
            }
        }

        static void BringToFront(int index)
        {
            Window w = windows[index];

            for (int i = index; i < count - 1; i++)
                windows[i] = windows[i + 1];

            windows[count - 1] = w;
        }
    }
}
