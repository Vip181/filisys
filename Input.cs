using Cosmos.System;

namespace filesys.Input
{
    public static class KeyboardLayoutFR
    {
        public static bool CapsLockEnabled = false;

        public static char Convert(ConsoleKeyEx key, char keyChar, bool shift)
        {
            if (keyChar == '\0') return '\0';

            if (keyChar >= 'a' && keyChar <= 'z')
            {
                bool upper = shift ^ CapsLockEnabled;
                return upper ? char.ToUpper(keyChar) : keyChar;
            }

            if (shift && keyChar >= '0' && keyChar <= '9')
                return keyChar;

            return keyChar;
        }
    }
}
