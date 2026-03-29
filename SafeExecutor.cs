using System;

namespace filesys.System
{
    public static class SafeExecutor
    {
        public static void Execute(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // Ignore crash
            }
        }
    }
}