using System;
using System.Collections.Generic;

namespace filesys.System.USB
{
    public static class USBKeyboardDriver
    {
        private static Queue<char> keyBuffer = new Queue<char>(256);

        private static bool shift;
        private static bool ctrl;

        // =========================
        // INIT
        // =========================
     

        // =========================
        // CALLED BY USB STACK (HID INTERRUPT IN)
        // =========================
        public static void OnHIDReport(byte[] report)
        {
            /*
             HID Keyboard report format:
             [0] modifiers (ctrl/shift/etc)
             [1] reserved
             [2..7] keycodes
            */

            byte modifiers = report[0];

            shift = (modifiers & 0x22) != 0; // left/right shift
            ctrl = (modifiers & 0x11) != 0; // ctrl bits

            for (int i = 2; i < report.Length; i++)
            {
                byte key = report[i];
                if (key == 0) continue;

                char c = TranslateKey(key, shift);

                if (ctrl)
                {
                    c = TranslateCtrl(key);
                }

                if (c != '\0')
                    Enqueue(c);
            }
        }

        // =========================
        // BUFFER
        // =========================
        private static void Enqueue(char c)
        {
            if (keyBuffer.Count >= 256)
                keyBuffer.Dequeue();

            keyBuffer.Enqueue(c);
        }

        public static bool TryRead(out char c)
        {
            if (keyBuffer.Count == 0)
            {
                c = '\0';
                return false;
            }

            c = keyBuffer.Dequeue();
            return true;
        }

        // =========================
        // USB KEYCODE MAP (US BASIC)
        // =========================
        private static char TranslateKey(byte key, bool shift)
        {
            switch (key)
            {
                case 0x04: return shift ? 'A' : 'a';
                case 0x05: return shift ? 'B' : 'b';
                case 0x06: return shift ? 'C' : 'c';
                case 0x07: return shift ? 'D' : 'd';
                case 0x08: return shift ? 'E' : 'e';
                case 0x09: return shift ? 'F' : 'f';
                case 0x0A: return shift ? 'G' : 'g';
                case 0x0B: return shift ? 'H' : 'h';
                case 0x0C: return shift ? 'I' : 'i';
                case 0x0D: return shift ? 'J' : 'j';
                case 0x0E: return shift ? 'K' : 'k';
                case 0x0F: return shift ? 'L' : 'l';
                case 0x10: return shift ? 'M' : 'm';
                case 0x11: return shift ? 'N' : 'n';
                case 0x12: return shift ? 'O' : 'o';
                case 0x13: return shift ? 'P' : 'p';
                case 0x14: return shift ? 'Q' : 'q';
                case 0x15: return shift ? 'R' : 'r';
                case 0x16: return shift ? 'S' : 's';
                case 0x17: return shift ? 'T' : 't';
                case 0x18: return shift ? 'U' : 'u';
                case 0x19: return shift ? 'V' : 'v';
                case 0x1A: return shift ? 'W' : 'w';
                case 0x1B: return shift ? 'X' : 'x';
                case 0x1C: return shift ? 'Y' : 'y';
                case 0x1D: return shift ? 'Z' : 'z';

                case 0x2C: return ' ';
                case 0x28: return '\n';

                case 0x2A: return '\b';

                default:
                    return '\0';
            }
        }

        // =========================
        // CTRL SHORTCUTS
        // =========================
        private static char TranslateCtrl(byte key)
        {
            return key switch
            {
                0x06 => '\x03', // C
                0x1B => '\x18', // X
                0x19 => '\x16', // V
                0x04 => '\x01', // A
                0x1D => '\x1A', // Z
                0x1C => '\x19', // Y
                _ => '\0'
            };
        }


        private static char TranslateAlt(byte key)
        {
            return key switch
            {
                0x04 => '\x1B', // A -> Alt+1
                0x05 => '\x1C', // B -> Alt+2
                0x06 => '\x1D', // C -> Alt+3
                0x07 => '\x1E', // D -> Alt+4
                0x08 => '\x1F', // E -> Alt+5
                _ => '\0'
            };
        }
        private static char TranslateShift(byte key)
        {
            return key switch
            {
                0x04 => 'A',
                0x05 => 'B',
                0x06 => 'C',
                0x07 => 'D',
                0x08 => 'E',
                0x09 => 'F',
                0x0A => 'G',
                0x0B => 'H',
                0x0C => 'I',
                0x0D => 'J',
                0x0E => 'K',
                0x0F => 'L',
                0x10 => 'M',
                0x11 => 'N',
                0x12 => 'O',
                0x13 => 'P',
                0x14 => 'Q',
                0x15 => 'R',
                0x16 => 'S',
                0x17 => 'T',
                0x18 => 'U',
                0x19 => 'V',
                0x1A => 'W',
                0x1B => 'X',
                0x1C => 'Y',
                0x1D => 'Z',
                _ => '\0'
            };
        }
    }
}