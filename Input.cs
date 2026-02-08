using Cosmos.System;

namespace filesys.Input
{
    public static class KeyboardLayoutFR
    {
        // Verr Maj logiciel
        public static bool CapsLockEnabled = false;

        public static char Convert(ConsoleKeyEx key, char keyChar, bool shift)
        {
            if (keyChar == '\0')
                return '\0';

            // =========================
            // LETTRES AZERTY
            // =========================
            if (keyChar >= 'a' && keyChar <= 'z')
            {
                char c = keyChar;

                // QWERTY -> AZERTY
                if (c == 'a') c = 'q';
                else if (c == 'q') c = 'a';
                else if (c == 'z') c = 'w';
                else if (c == 'w') c = 'z';

                bool upper = shift ^ CapsLockEnabled;
                return upper ? char.ToUpper(c) : c;
            }

            // =========================
            // CHIFFRES AZERTY
            // =========================
            if (!shift)
            {
                switch (keyChar)
                {
                    case '1': return '&';
                    case '2': return 'é';
                    case '3': return '"';
                    case '4': return '\'';
                    case '5': return '(';
                    case '6': return '-';
                    case '7': return 'è';
                    case '8': return '_';
                    case '9': return 'ç';
                    case '0': return 'à';
                }
            }
            else
            {
                if (keyChar >= '0' && keyChar <= '9')
                    return keyChar;
            }

            // =========================
            // PONCTUATION (STABLE)
            // =========================
            if (keyChar == '.') return '.';
            if (keyChar == ',') return ',';
            if (keyChar == ';') return ';';
            if (keyChar == ':') return ':';
            if (keyChar == '!') return '!';
            if (keyChar == '?') return '?';
            if (keyChar == '/') return '/';
            if (keyChar == '-') return shift ? '_' : '-';
            if (keyChar == '=') return shift ? '+' : '=';
            if (keyChar == ' ') return ' ';

            return keyChar;
        }
    }
}
