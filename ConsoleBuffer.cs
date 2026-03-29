using System;

namespace filesys.System
{
    public class ConsoleBuffer
    {
        private string[] lines;
        private int capacity;
        private int writeIndex = 0;
        private int count = 0;

        public ConsoleBuffer(int maxLines)
        {
            capacity = maxLines;
            lines = new string[capacity];
        }

        public void WriteLine(string text)
        {
            try
            {
                if (text == null)
                    text = "";

                if (text.Length > 256)
                    text = text.Substring(0, 256);

                lines[writeIndex] = text;

                writeIndex++;
                if (writeIndex >= capacity)
                    writeIndex = 0;

                if (count < capacity)
                    count++;
            }
            catch
            {
                // ignore crash
            }
        }

        public void Clear()
        {
            lines = new string[capacity];
            writeIndex = 0;
            count = 0;
        }

        public string[] GetLines()
        {
            string[] result = new string[count];

            int index = writeIndex - count;
            if (index < 0)
                index += capacity;

            for (int i = 0; i < count; i++)
            {
                result[i] = lines[(index + i) % capacity];
            }

            return result;
        }
    }
}