using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Sys = Cosmos.System;
using filesys.System;
using Cosmos.System.FileSystem.VFS;

namespace filesys
{
    public class Kernel : Sys.Kernel
    {
        public static Kernel Instance;
        public static WindowManager WindowMgr;
        private Canvas canvas;

        public List<BaseWindow> windows = new List<BaseWindow>();

        private bool menuOpen = false;
        private int menuWidth = 200;
        private int menuHeight = 350;

        // Confirmation shutdown
        private bool confirmPower = false;
        private bool confirmShutdown = true;
       
        public List<BaseWindow> GetWindows() => windows;
        protected override void BeforeRun()
        {
            Instance = this;

            var (w, h) = Config.LoadResolution();
            ScreenManager.Init(w, h);
            canvas = ScreenManager.Canvas;

            WindowMgr = new WindowManager();

            Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
            VFSManager.RegisterVFS(fs);

            AddWindow(new WindowsConsole(100, 100));
        }

        protected override void Run()
        {
            UpdateSystem();

            canvas.Clear(Color.CornflowerBlue);
            WindowMgr.Draw(canvas);

            if (menuOpen) DrawStartMenu();
            DrawTaskbar();

            // Curseur souris
            canvas.DrawFilledRectangle(
                new Pen(Color.White),
                (int)MouseManager.X,
                (int)MouseManager.Y,
                8,
                8
            );

            canvas.Display();
            Cosmos.Core.Memory.Heap.Collect();
        }

        public void AddWindow(BaseWindow w) => windows.Add(w);

        private void UpdateSystem()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;
            int menuY = taskbarY - menuHeight;

            if (click)
            {
                // CONFIRMATION POWER
                if (confirmPower)
                {
                    if (mx > 40 && mx < 120 && my > menuY + 220 && my < menuY + 250)
                    {
                        if (confirmShutdown) Sys.Power.Shutdown();
                        else Sys.Power.Reboot();
                    }
                    if (mx > 140 && mx < 220 && my > menuY + 220 && my < menuY + 250)
                        confirmPower = false;

                    Thread.Sleep(200);
                    return;
                }

                // MENU START
                if (menuOpen && mx <= menuWidth && my >= menuY && my <= taskbarY)
                {
                    if (my > menuY + 20 && my < menuY + 80)
                        AddWindow(new WindowsConsole(150, 150));
                    else if (my > menuY + 80 && my < menuY + 140)
                        AddWindow(new FileExplorerWindow(150, 150));
                    else if (my > menuY + 140 && my < menuY + 200)
                        AddWindow(new TaskManager(150, 150));
                    else if (my > taskbarY - 40)
                    {
                        confirmPower = true;
                        confirmShutdown = mx < menuWidth / 2;
                    }

                    menuOpen = false;
                    Thread.Sleep(200);
                }
                else if (my >= taskbarY && mx <= 50)
                {
                    menuOpen = !menuOpen;
                    Thread.Sleep(200);
                }
                else menuOpen = false;
            }

            for (int i = windows.Count - 1; i >= 0; i--)
            {
                var w = windows[i];
                if (!w.IsMinimized) w.Update();
                if (w.IsClosed) windows.RemoveAt(i);
            }
        }

        private void DrawStartMenu()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;
            int menuY = taskbarY - menuHeight;

            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(220, 220, 220)), 0, menuY, menuWidth, menuHeight);
            canvas.DrawRectangle(new Pen(Color.Black), 0, menuY, menuWidth, menuHeight);

            DrawMenuItem(10, menuY + 25, "Console", DrawConsoleIcon, mx, my);
            DrawMenuItem(10, menuY + 85, "Files", DrawFilesIcon, mx, my);
            DrawMenuItem(10, menuY + 145, "Tasks", DrawTasksIcon, mx, my);

            // Boutons power
            int btnW = menuWidth / 2;
            canvas.DrawFilledRectangle(new Pen(Color.DarkRed), 0, taskbarY - 40, btnW, 40);
            canvas.DrawFilledRectangle(new Pen(Color.DarkOrange), btnW, taskbarY - 40, btnW, 40);

            canvas.DrawString("Shutdown", PCScreenFont.Default, new Pen(Color.White), 10, taskbarY - 25);
            canvas.DrawString("Restart", PCScreenFont.Default, new Pen(Color.White), btnW + 15, taskbarY - 25);

            if (confirmPower)
                DrawConfirmBox(menuY);
        }

        private void DrawConfirmBox(int menuY)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Black), 20, menuY + 200, 220, 80);
            canvas.DrawString("Are you sure ?", PCScreenFont.Default, new Pen(Color.White), 40, menuY + 210);

            canvas.DrawFilledRectangle(new Pen(Color.Green), 40, menuY + 230, 80, 20);
            canvas.DrawFilledRectangle(new Pen(Color.Red), 140, menuY + 230, 80, 20);

            canvas.DrawString("Yes", PCScreenFont.Default, new Pen(Color.White), 65, menuY + 233);
            canvas.DrawString("No", PCScreenFont.Default, new Pen(Color.White), 170, menuY + 233);
        }

        private void DrawMenuItem(int x, int y, string text, Action<int, int> icon, int mx, int my)
        {
            bool hover = mx >= x && mx <= x + 180 && my >= y && my <= y + 40;

            if (hover)
                canvas.DrawFilledRectangle(new Pen(Color.LightBlue), 0, y - 5, menuWidth, 45);

            icon(x, y);
            canvas.DrawString(text, PCScreenFont.Default, new Pen(Color.Black), 60, y + 10);
        }

        private void DrawTaskbar()
        {
            int screenWidth = (int)canvas.Mode.Columns;
            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;

            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(30, 30, 30)), 0, taskbarY, screenWidth, 30);
            canvas.DrawFilledRectangle(new Pen(Color.White), 5, taskbarY + 5, 40, 20);
            canvas.DrawString("OS", PCScreenFont.Default, new Pen(Color.Black), 15, taskbarY + 8);
        }

        // 🎨 ICÔNES COULEUR
        private void DrawConsoleIcon(int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Black), x, y, 40, 25);
            canvas.DrawString(">_", PCScreenFont.Default, new Pen(Color.Lime), x + 8, y + 6);
        }

        private void DrawFilesIcon(int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Goldenrod), x, y + 5, 40, 20);
            canvas.DrawFilledRectangle(new Pen(Color.Khaki), x + 5, y, 20, 8);
        }

        private void DrawTasksIcon(int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.SteelBlue), x, y, 40, 25);
            canvas.DrawLine(new Pen(Color.White), x + 5, y + 8, x + 35, y + 8);
            canvas.DrawLine(new Pen(Color.White), x + 5, y + 15, x + 25, y + 15);
        }
    }
}