using Cosmos.System;
using filesys.System;

namespace filesys.GUI
{
    public static class CursorManager
    {
        private static int blinkCounter = 0;
        private static bool visible = true;

        private const int BLINK_SPEED = 30;

        public static void Update()
        {
            blinkCounter++;

            if (blinkCounter > BLINK_SPEED)
            {
                visible = !visible;
                blinkCounter = 0;
               
            }
        }

        public static void Draw(int x, int y)
        {
            if (!visible) return;

            GraphicsManager.DrawString("_", x, y, GraphicsManager.WhitePen);
        }
    }
}
