using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.GUI;
using filesys.System;
using System;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Threading;
using Sys = Cosmos.System;

namespace filesys
{
    public class Kernel : Sys.Kernel
    {
        public static Kernel Instance;
        public static WindowManager WindowMgr;
        public static DesktopManager Desktop;

        private Canvas canvas;
        private UiBitmapCache taskbarCache;
        private UiBitmapCache startMenuCache;

        private bool taskbarDirty = true;
        private bool startMenuDirty = true;

        private static readonly Pen WhitePen = new Pen(Color.White);
        private static readonly Pen BlackPen = new Pen(Color.Black);
        private bool menuOpen = false;
        private int menuWidth = 200;
        private int menuHeight = 380;

        private const int DefaultColorIndex = 0;
        private const int DefaultWidth = 1920;
        private const int DefaultHeight = 1080;
        private const string ScreenConfigFile = @"0:\screen.cfg";

        private bool darkTheme = false;
        private bool confirmPower = false;
        private bool confirmShutdown = true;

        private const int ConfirmWidth = 220;
        private const int ConfirmHeight = 80;

        protected override void BeforeRun()
        {
            Instance = this;

            var fs = new Sys.FileSystem.CosmosVFS();
            VFSManager.RegisterVFS(fs);

            EnsureConfigFile();

            int width = DefaultWidth;
            int height = DefaultHeight;

            try
            {
                string[] cfg = File.ReadAllLines(ScreenConfigFile);
                if (cfg.Length >= 2)
                {
                    string[] res = cfg[1].Split(',');
                    width = int.Parse(res[0]);
                    height = int.Parse(res[1]);
                }
            }
            catch { }

            ScreenManager.Init(width, height);
            canvas = ScreenManager.Canvas;

            WindowMgr = new WindowManager();
            FilSysStudioWindow.WindowMgr = WindowMgr;

            Desktop = new DesktopManager();

            DesktopManager.WindowMgr = WindowMgr;
            CursorRenderer.Init();
            Desktop.Refresh();
            taskbarCache = new UiBitmapCache(ScreenManager.Width, 30);
            startMenuCache = new UiBitmapCache(menuWidth, menuHeight);
            LoadScreenColor();
            StartMenuIcons.Init();
        

        }
        private int gcCounter = 0;
      
        protected override void Run()
        {
            canvas = ScreenManager.Canvas;

            UpdateSystem();

            canvas.Clear(darkTheme ? Color.FromArgb(20, 20, 20) : StyleManager.DesktopBackgroundColor);
            Desktop.Update();
            Desktop.Draw(canvas);
      


            WindowMgr.Update();
            WindowMgr.Draw(canvas);

            if (confirmPower)
                DrawBackgroundBlur();

            if (menuOpen)
                DrawStartMenu();

            if (confirmPower)
                DrawConfirmBox();

            DrawTaskbar();
            CursorRenderer.Draw(canvas);

           

            canvas.Display();
        

          
            

        }

        public void AddWindow(BaseWindow window)
        {
            WindowMgr.Add(window);
        }

        private void UpdateSystem()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;
            int menuY = taskbarY - menuHeight;

            if (!click)
                return;

            if (confirmPower)
            {
                int screenWidth = (int)canvas.Mode.Columns;
                int boxX = (screenWidth / 2) - (ConfirmWidth / 2);
                int boxY = (screenHeight / 2) - (ConfirmHeight / 2);

                if (mx >= boxX + 20 && mx <= boxX + 90 &&
                    my >= boxY + 45 && my <= boxY + 65)
                {
                    if (confirmShutdown)
                        Sys.Power.Shutdown();
                    else
                        Sys.Power.Reboot();
                }

                if (mx >= boxX + 130 && mx <= boxX + 200 &&
                    my >= boxY + 45 && my <= boxY + 65)
                {
                    confirmPower = false;
                    Thread.Sleep(150);
                }

                return;
            }

            if (menuOpen && mx <= menuWidth && my >= menuY && my <= taskbarY)
            {
                if (my > menuY + 20 && my < menuY + 80)
                    WindowMgr.Add(new WindowsConsole(150, 150));
                else if (my > menuY + 80 && my < menuY + 140)
                    WindowMgr.Add(new FileExplorerWindow(150, 150));
                else if (my > menuY + 140 && my < menuY + 200)
                    WindowMgr.Add(new TaskManagerWindow(WindowMgr, 40, 40));
                else if (my > menuY + 200 && my < menuY + 250)
                    darkTheme = !darkTheme;
                else if (my > menuY + 260 && my < menuY + 320)
                    WindowMgr.Add(new ScreenSettingsWindow(200, 150));
                else if (my > taskbarY - 40)
                {
                    confirmPower = true;
                    confirmShutdown = mx < menuWidth / 2;
                }

                menuOpen = false;
                Thread.Sleep(150);
            }
            else if (my >= taskbarY && mx <= 50)
            {
               
                menuOpen = !menuOpen;
                Thread.Sleep(150);
            }
            else
            {
                menuOpen = false;
            }
        }

        private void DrawTaskbar()
        {
            int screenWidth = (int)canvas.Mode.Columns;
            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;

            if (taskbarDirty)
            {
                taskbarCache = new UiBitmapCache(screenWidth, 30);
                taskbarCache.Clear(Color.FromArgb(20, 20, 20));

                taskbarCache.FillRect(5, 5, 40, 20, Color.White);
                taskbarCache.Border(5, 5, 40, 20, Color.Gray);

                taskbarCache.Apply();

                taskbarDirty = false;
            }

            canvas.DrawImage(taskbarCache.Bitmap, 0, taskbarY);

            canvas.DrawString("OS", PCScreenFont.Default, BlackPen, 15, taskbarY + 8);

            int x = 60;
            var wins = WindowMgr.GetWindows();

            for (int i = 0; i < wins.Count; i++)
            {
                BaseWindow w = wins[i];

                if (w == null || w.IsClosed)
                    continue;

                int width = 120;

                canvas.DrawFilledRectangle(
                    new Pen(w.IsMinimized ? Color.FromArgb(70, 120, 200) : Color.FromArgb(140, 140, 140)),
                    x,
                    taskbarY + 5,
                    width,
                    20
                );

                canvas.DrawString(w.Title, PCScreenFont.Default, WhitePen, x + 5, taskbarY + 8);

                x += width + 5;
            }
        }

        private void DrawStartMenu()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;
            int menuY = taskbarY - menuHeight;

            if (startMenuDirty)
            {
                startMenuCache.Border(0, 0, menuWidth, menuHeight, Color.Black);
                startMenuCache = new UiBitmapCache(menuWidth, menuHeight);
               
                   Color bg = Color.FromArgb(36, 36, 36);

                startMenuCache.Clear(bg);
                startMenuCache.Border(0, 0, menuWidth, menuHeight, Color.Black);

                startMenuCache.FillRect(0, menuHeight - 40, menuWidth / 2, 40, Color.DarkRed);
                startMenuCache.FillRect(menuWidth / 2, menuHeight - 40, menuWidth / 2, 40, Color.DarkOrange);

                startMenuCache.Apply();

                startMenuDirty = false;
            }

            canvas.DrawImage(startMenuCache.Bitmap, 0, menuY);

            Color text = darkTheme ? Color.White : Color.White;
            int iconX = 10;
            int textX = 55;

            int startY = menuY + 20;
            int spacing = 55;

            // Console
            canvas.DrawImageAlpha(StartMenuIcons.Console, iconX, startY);
            canvas.DrawString("Console", PCScreenFont.Default, new Pen(text), textX, startY + 8);

            // Files
            canvas.DrawImageAlpha(StartMenuIcons.Explorer, iconX, startY + spacing);
            canvas.DrawString("Files", PCScreenFont.Default, new Pen(text), textX, startY + spacing + 8);

            // Tasks
            canvas.DrawImageAlpha(StartMenuIcons.TaskManager, iconX, startY + spacing * 2);
            canvas.DrawString("Tasks", PCScreenFont.Default, new Pen(text), textX, startY + spacing * 2 + 8);

            // Theme
            canvas.DrawImageAlpha(StartMenuIcons.Theme, iconX, startY + spacing * 3);
            canvas.DrawString("Theme", PCScreenFont.Default, new Pen(text), textX, startY + spacing * 3 + 8);

            // Screen Settings
            canvas.DrawImageAlpha(StartMenuIcons.Settings, iconX, startY + spacing * 4);
            canvas.DrawString("Screen Settings", PCScreenFont.Default, new Pen(text), textX, startY + spacing * 4 + 8);
        }

        private void DrawMenuItem(int x, int y, string text, Action<int, int> icon, Color textColor, Color hoverColor, int mx, int my)
        {
            bool hover = mx >= 0 && mx <= menuWidth && my >= y && my <= y + 45;

            if (hover)
                canvas.DrawFilledRectangle(new Pen(hoverColor), 0, y - 5, menuWidth, 45);

            icon(x, y);
            canvas.DrawString(text, PCScreenFont.Default, new Pen(textColor), 60, y + 10);
        }

        private void DrawConfirmBox()
        {
            int screenWidth = (int)canvas.Mode.Columns;
            int screenHeight = (int)canvas.Mode.Rows;

            int x = (screenWidth / 2) - (ConfirmWidth / 2);
            int y = (screenHeight / 2) - (ConfirmHeight / 2);

            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(200, 40, 80, 160)), x, y, ConfirmWidth, ConfirmHeight);
            canvas.DrawRectangle(new Pen(Color.White), x, y, ConfirmWidth, ConfirmHeight);

            canvas.DrawString(confirmShutdown ? "Shutdown ?" : "Restart ?", PCScreenFont.Default, new Pen(Color.White), x + 55, y + 10);

            canvas.DrawFilledRectangle(new Pen(Color.Green), x + 20, y + 45, 70, 20);
            canvas.DrawString("Yes", PCScreenFont.Default, new Pen(Color.White), x + 40, y + 48);

            canvas.DrawFilledRectangle(new Pen(Color.Red), x + 130, y + 45, 70, 20);
            canvas.DrawString("No", PCScreenFont.Default, new Pen(Color.White), x + 155, y + 48);
        }

        private void DrawBackgroundBlur()
        {
            int w = (int)canvas.Mode.Columns;
            int h = (int)canvas.Mode.Rows;

            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(120, 0, 0, 0)), 0, 0, w, h);
        }

        private void EnsureConfigFile()
        {
            if (File.Exists(ScreenConfigFile))
                return;

            try
            {
                File.WriteAllLines(ScreenConfigFile, new string[]
                {
                    DefaultColorIndex.ToString(),
                    DefaultWidth + "," + DefaultHeight
                });
            }
            catch { }
        }

        private void LoadScreenColor()
        {
            try
            {
                string[] cfg = File.ReadAllLines(ScreenConfigFile);

                if (cfg.Length >= 1)
                {
                    int index = int.Parse(cfg[0]);
                    StyleManager.DesktopBackgroundColor = StyleManager.GetColorFromIndex(index);
                }
            }
            catch
            {
                StyleManager.DesktopBackgroundColor = Color.Gray;
            }
        }

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

        private void DrawScreenSettingsIcon(int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.DimGray), x, y, 40, 25);
            canvas.DrawRectangle(new Pen(Color.White), x + 4, y + 4, 14, 10);
            canvas.DrawRectangle(new Pen(Color.White), x + 22, y + 4, 14, 10);
            canvas.DrawLine(new Pen(Color.Lime), x + 6, y + 18, x + 34, y + 18);
            canvas.DrawFilledRectangle(new Pen(Color.Lime), x + 18, y + 15, 4, 6);
        }
    }
}