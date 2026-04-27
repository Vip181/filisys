using System;
using System.Collections.Generic;

namespace filesys.System
{
    public class ConsoleBuffer
    {
        private List<string> lines = new List<string>();

        private int maxLines;

        public ConsoleBuffer(int max)
        {
            maxLines = max;
        }

        // =========================================================
        // WRITE
        // =========================================================
        public void WriteLine(string text)
        {
            if (text == null) text = "";

            lines.Add(text);

            // ⚡ sécurité mémoire Cosmos
            if (lines.Count > maxLines)
                lines.RemoveAt(0);
        }

        // =========================================================
        // CLEAR
        // =========================================================
        public void Clear()
        {
            lines.Clear();
        }

        // =========================================================
        // GET
        // =========================================================
        public string[] GetLines()
        {
            return lines.ToArray();
        }

        // =========================================================
        // REPLACE ALL LINES (FIX POUR CUT)
        // =========================================================
        public void SetLines(string[] newLines)
        {
            lines.Clear();

            if (newLines == null) return;

            int limit = Math.Min(newLines.Length, maxLines);

            for (int i = 0; i < limit; i++)
            {
                if (newLines[i] != null)
                    lines.Add(newLines[i]);
            }
        }
    }
}