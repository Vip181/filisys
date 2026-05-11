using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace filesys.GUI
{
    public class TaskManagerWindow : BaseWindow
    {
        private WindowManager windowManager;

        private bool lastClick = false;

        private int frameCounter = 0;
        private int fps = 0;
        private int lastSecond = 0;

        private static Pen White = new Pen(Color.White);
        private static Pen Gray = new Pen(Color.Gray);
        private static Pen Dark = new Pen(Color.FromArgb(35, 35, 35));
        private static Pen Header = new Pen(Color.FromArgb(45, 45, 45));
        private static Pen Green = new Pen(Color.Lime);
        private static Pen Red = new Pen(Color.Red);
        private static Pen Yellow = new Pen(Color.Yellow);
        WindowManager WindowMgr;
        public TaskManagerWindow(WindowManager manager, int x, int y)
            : base("Task Manager", x, y, 420, 360)
        {
            windowManager = manager;
        }

        public override void Update()
        {
            base.Update();

            if (IsClosed || IsMinimized)
                return;

            UpdateFPS();

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool click = MouseManager.MouseState == MouseState.Left;

            if (click && !lastClick)
            {
                KillProcessClick(mx, my);
            }

            lastClick = click;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (IsClosed || IsMinimized)
                return;

            DrawStats(canvas);
            DrawProcessList(canvas);
        }

        private void DrawStats(Canvas canvas)
        {
            int usedMb = (int)(GCImplementation.GetUsedRAM() / 1024 / 1024);
            int totalMb = 253;

            canvas.DrawString("CPU : " + GetFakeCpuPercent() + "%", PCScreenFont.Default, White, X + 15, Y + 40);
            canvas.DrawString("FPS : " + fps, PCScreenFont.Default, White, X + 120, Y + 40);
            canvas.DrawString("GC  : " + usedMb + " MB / " + totalMb + " MB", PCScreenFont.Default, White, X + 220, Y + 40);

            canvas.DrawRectangle(Gray, X + 15, Y + 60, 180, 14);

            int barWidth = usedMb * 180 / totalMb;
            if (barWidth > 180)
                barWidth = 180;

            canvas.DrawFilledRectangle(Green, X + 15, Y + 60, barWidth, 14);
        }

        private void DrawProcessList(Canvas canvas)
        {
            int startY = Y + 90;

            canvas.DrawFilledRectangle(Header, X + 10, startY, Width - 20, 22);

            canvas.DrawString("PROCESSUS", PCScreenFont.Default, White, X + 15, startY + 6);
            canvas.DrawString("RAM", PCScreenFont.Default, White, X + 180, startY + 6);
            canvas.DrawString("PID", PCScreenFont.Default, White, X + 240, startY + 6);
            canvas.DrawString("KILL", PCScreenFont.Default, White, X + 310, startY + 6);

            int y = startY + 30;

            List<BaseWindow> windows = windowManager.GetWindows();

            for (int i = 0; i < windows.Count; i++)
            {
                BaseWindow win = windows[i];

                if (win == null)
                    continue;

                canvas.DrawFilledRectangle(Dark, X + 10, y - 2, Width - 20, 20);

                canvas.DrawString(win.Title, PCScreenFont.Default, Green, X + 15, y);
                canvas.DrawString(GetWindowMemory(win) + " MB", PCScreenFont.Default, White, X + 180, y);
                canvas.DrawString("#" + i, PCScreenFont.Default, Gray, X + 240, y);

                canvas.DrawFilledRectangle(Red, X + 310, y - 3, 55, 18);
                canvas.DrawString("Kill", PCScreenFont.Default, White, X + 320, y);

                y += 24;

                if (y > Y + Height - 20)
                    break;
            }
        }

        private void KillProcessClick(int mx, int my)
        {
            int startY = Y + 120;
            List<BaseWindow> windows = windowManager.GetWindows();

            for (int i = 0; i < windows.Count; i++)
            {
                int y = startY + i * 24;

                bool onKill =
                    mx >= X + 310 &&
                    mx <= X + 365 &&
                    my >= y - 3 &&
                    my <= y + 15;

                if (onKill)
                {
                    BaseWindow win = windows[i];

                    if (win == this)
                        return;

                    win.IsClosed = true;
                    return;
                }
            }
        }

        private int GetWindowMemory(BaseWindow win)
        {
            if (win.Title == "FilSys Studio")
                return 12;

            if (win.Title == "Task Manager")
                return 3;

            return 5;
        }

        private int GetFakeCpuPercent()
        {
            int cpu = windowManager.Count * 8;

            if (cpu < 1)
                cpu = 1;

            if (cpu > 100)
                cpu = 100;

            return cpu;
        }

        private void UpdateFPS()
        {
            frameCounter++;

            int second = DateTime.Now.Second;

            if (second != lastSecond)
            {
                fps = frameCounter;
                frameCounter = 0;
                lastSecond = second;
            }
        }
    }
}