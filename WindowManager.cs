using Cosmos.System.Graphics;
using System.Collections.Generic;

namespace filesys.GUI
{
    public class WindowManager
    {
        private List<BaseWindow> windows = new List<BaseWindow>();

        public int Count
        {
            get { return windows.Count; }
        }

        public void Add(BaseWindow window)
        {
            if (window == null)
                return;

            if (windows.Count >= 8)
                return;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].Title == window.Title)
                {
                    windows[i].IsMinimized = false;
                    return;
                }
            }

            windows.Add(window);
        }

        public List<BaseWindow> GetWindows()
        {
            return windows;
        }

        public void Update()
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                BaseWindow w = windows[i];

                if (w == null || w.IsClosed)
                {
                    windows.RemoveAt(i);
                    continue;
                }

                if (!w.IsMinimized)
                    w.Update();
            }
        }

        public void Draw(Canvas canvas)
        {
            if (canvas == null)
                return;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i] == null)
                    continue;

                if (windows[i].IsClosed)
                    continue;

                windows[i].Draw(canvas);
            }
        }
    }
}