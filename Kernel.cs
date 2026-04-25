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
        public static DesktopManager Desktop;
        public List<BaseWindow> windows = new List<BaseWindow>();

        private bool menuOpen = false;
        private int menuWidth = 200;
        private int menuHeight = 380;

        // 🌗 THÈME
        private bool darkTheme = false;

        // 🔔 confirmation power
        private bool confirmPower = false;
        private bool confirmShutdown = true;

        // 📦 Confirm box size
        private const int ConfirmWidth = 220;
        private const int ConfirmHeight = 80;

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
            Desktop = new DesktopManager();
            Desktop.Refresh();
        }

        protected override void Run()
        {
            UpdateSystem();

            canvas.Clear(darkTheme ? Color.FromArgb(20, 20, 20) : Color.CornflowerBlue);
            Desktop.Update();
            Desktop.Draw(canvas);
            WindowMgr.Draw(canvas);

            // 🌫️ blur derrière la combox
            if (confirmPower)
                DrawBackgroundBlur();

            if (menuOpen)
                DrawStartMenu();

            // ✅ combox dessinée même si le menu est fermé
            if (confirmPower)
            {
                int screenHeight = (int)canvas.Mode.Rows;
                int taskbarY = screenHeight - 30;
                int menuY = taskbarY - menuHeight;
                DrawConfirmBox(menuY);
            }

            DrawTaskbar();

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
                if (confirmPower)
                {
                    int screenWidth = (int)canvas.Mode.Columns;
                    int screenHeighte = (int)canvas.Mode.Rows;

                    int boxX = (screenWidth / 2) - (ConfirmWidth / 2);
                    int boxY = (screenHeighte / 2) - (ConfirmHeight / 2);

                    // ✅ BOUTON YES
                    if (mx >= boxX + 20 && mx <= boxX + 90 &&
                        my >= boxY + 45 && my <= boxY + 65)
                    {
                        if (confirmShutdown)
                            Sys.Power.Shutdown();
                        else
                            Sys.Power.Reboot();

                        Thread.Sleep(200);
                    }

                    // ✅ BOUTON NO
                    if (mx >= boxX + 130 && mx <= boxX + 200 &&
                        my >= boxY + 45 && my <= boxY + 65)
                    {
                        confirmPower = false;
                        Thread.Sleep(200);
                    }

                    return;
                }

                if (menuOpen && mx <= menuWidth && my >= menuY && my <= taskbarY)
                {
                    if (my > menuY + 20 && my < menuY + 80)
                        AddWindow(new WindowsConsole(150, 150));
                    else if (my > menuY + 80 && my < menuY + 140)
                        AddWindow(new FileExplorerWindow(150, 150));
                    else if (my > menuY + 140 && my < menuY + 200)
                        AddWindow(new TaskManager(150, 150));
                    else if (my > menuY + 200 && my < menuY + 250)
                        darkTheme = !darkTheme;
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

        private void ToggleWindow(BaseWindow w)
        {
            w.IsMinimized = !w.IsMinimized;
        }
        public void AddWindow(BaseWindow w) => windows.Add(w);
        // 🌫️ BACKGROUND BLUR
        private void DrawBackgroundBlur()
        {
            int w = (int)canvas.Mode.Columns;
            int h = (int)canvas.Mode.Rows;

            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(120, 0, 0, 0)),
                0, 0, w, h
            );
        }

        private void DrawStartMenu()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;
            int menuY = taskbarY - menuHeight;

            Color bg = darkTheme ? Color.FromArgb(40, 40, 40) : Color.FromArgb(230, 230, 230);
            Color text = darkTheme ? Color.White : Color.Black;
            Color hover = darkTheme ? Color.FromArgb(70, 70, 70) : Color.LightBlue;

            canvas.DrawFilledRectangle(new Pen(bg), 0, menuY, menuWidth, menuHeight);
            canvas.DrawRectangle(new Pen(Color.Black), 0, menuY, menuWidth, menuHeight);

            DrawMenuItem(10, menuY + 25, "Console", DrawConsoleIcon, text, hover, mx, my);
            DrawMenuItem(10, menuY + 85, "Files", DrawFilesIcon, text, hover, mx, my);
            DrawMenuItem(10, menuY + 145, "Tasks", DrawTasksIcon, text, hover, mx, my);
            DrawMenuItem(10, menuY + 205, "Theme", DrawThemeIcon, text, hover, mx, my);

            int btnW = menuWidth / 2;
            canvas.DrawFilledRectangle(new Pen(Color.DarkRed), 0, taskbarY - 40, btnW, 40);
            canvas.DrawFilledRectangle(new Pen(Color.DarkOrange), btnW, taskbarY - 40, btnW, 40);

            canvas.DrawString("Shutdown", PCScreenFont.Default, new Pen(Color.White), 10, taskbarY - 25);
            canvas.DrawString("Restart", PCScreenFont.Default, new Pen(Color.White), btnW + 15, taskbarY - 25);
        }

        // 🔵 COMBOX BLEUE SEMI-TRANSPARENTE
        private void DrawConfirmBox(int menuY)
        {
            int screenWidth = (int)canvas.Mode.Columns;
            int screenHeight = (int)canvas.Mode.Rows;

            // ✅ CENTRAGE PARFAIT ÉCRAN
            int x = (screenWidth / 2) - (ConfirmWidth / 2);
            int y = (screenHeight / 2) - (ConfirmHeight / 2);

            canvas.DrawFilledRectangle(
                new Pen(Color.FromArgb(200, 40, 80, 160)), // bleu semi-transparent
                x, y, ConfirmWidth, ConfirmHeight
            );

            canvas.DrawRectangle(new Pen(Color.White), x, y, ConfirmWidth, ConfirmHeight);

            canvas.DrawString(
                confirmShutdown ? "Shutdown ?" : "Restart ?",
                PCScreenFont.Default,
                new Pen(Color.White),
                x + 55, y + 10
            );

            canvas.DrawFilledRectangle(new Pen(Color.Green), x + 20, y + 45, 70, 20);
            canvas.DrawString("Yes", PCScreenFont.Default, new Pen(Color.White), x + 40, y + 48);

            canvas.DrawFilledRectangle(new Pen(Color.Red), x + 130, y + 45, 70, 20);
            canvas.DrawString("No", PCScreenFont.Default, new Pen(Color.White), x + 155, y + 48);
        }

        private void DrawMenuItem(
            int x, int y,
            string text,
            Action<int, int> icon,
            Color textColor,
            Color hoverColor,
            int mx, int my)
        {
            bool hover = mx >= 0 && mx <= menuWidth && my >= y && my <= y + 45;
            if (hover)
                canvas.DrawFilledRectangle(new Pen(hoverColor), 0, y - 5, menuWidth, 45);

            icon(x, y);
            canvas.DrawString(text, PCScreenFont.Default, new Pen(textColor), 60, y + 10);
        }

        private void DrawTaskbar()
        {
            int screenWidth = (int)canvas.Mode.Columns;
            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;

            Color bg = darkTheme ? Color.FromArgb(30, 30, 30) : Color.FromArgb(20, 20, 20);
            Color txt = darkTheme ? Color.White : Color.Black;

            canvas.DrawFilledRectangle(new Pen(bg), 0, taskbarY, screenWidth, 30);

            // START BUTTON
            canvas.DrawFilledRectangle(new Pen(Color.White), 5, taskbarY + 5, 40, 20);
            canvas.DrawString("OS", PCScreenFont.Default, new Pen(txt), 15, taskbarY + 8);

            // 🪟 WINDOWS (MINIMIZED + NORMAL)
            int x = 60;

            for (int i = 0; i < windows.Count; i++)
            {
                var w = windows[i];
                if (w.IsClosed) continue;

                int width = 120;

                bool hover =
                    MouseManager.X >= x &&
                    MouseManager.X <= x + width &&
                    MouseManager.Y >= taskbarY &&
                    MouseManager.Y <= taskbarY + 30;

                Color btnColor;

                if (w.IsMinimized)
                    btnColor = Color.FromArgb(70, 120, 200); // bleu (minimized)
                else
                    btnColor = Color.FromArgb(140, 140, 140);

                if (hover)
                    btnColor = Color.FromArgb(200, 200, 200);

                canvas.DrawFilledRectangle(new Pen(btnColor), x, taskbarY + 5, width, 20);

                canvas.DrawString(
                    w.Title,
                    PCScreenFont.Default,
                    new Pen(Color.White),
                    x + 5,
                    taskbarY + 8
                );

                // 🧠 CLICK RESTORE / MINIMIZE
                if (hover && MouseManager.MouseState == MouseState.Left)
                {
                    w.IsMinimized = !w.IsMinimized;

                    // si on restaure → remettre au-dessus
                    if (!w.IsMinimized)
                    {
                        windows.Remove(w);
                        windows.Add(w);
                    }

                    Thread.Sleep(150);
                }

                x += width + 5;
            }
        }

        // 🎨 ICONS inchangés
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

        private void DrawThemeIcon(int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(darkTheme ? Color.Black : Color.White), x + 5, y + 5, 15, 15);
            canvas.DrawFilledRectangle(new Pen(darkTheme ? Color.White : Color.Black), x + 20, y + 5, 15, 15);
        }
    }
}