using Cosmos.System.Graphics;
using System.Collections.Generic;

namespace filesys.GUI
{
    public class WindowManager
    {
        private List<BaseWindow> windows = new List<BaseWindow>();

        public void Add(BaseWindow window)
        {
            windows.Add(window);
        }

        public void Update()
        {
            foreach (var w in windows.ToArray())
            {
                filesys.System.SafeExecutor.Execute(() =>
                {
                    w.Update();
                });

                if (w.IsClosed)
                    windows.Remove(w);
            }
        }

        public void Draw(Canvas canvas)
        {
            foreach (var w in windows)
            {
                filesys.System.SafeExecutor.Execute(() =>
                {
                    w.Draw(canvas);
                });
            }
        }
    }
}