using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.System;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace filesys.GUI
{
    public class WindowsConsole : BaseWindow
    {
        private ConsoleBuffer buffer;
        private string input = "";
        private int lineHeight = 16;

        // ===== CURSEUR =====
        private int cursorIndex = 0;

        // ===== CLIPBOARD =====
        private string clipboard = "";

        // ===== SELECTION =====
        private int selectionStart = -1;
        private int selectionEnd = -1;

        // ===== SCROLL =====
        private int scrollOffset = 0;
        private bool draggingScroll = false;

        private ProcessMemoryManager memManager;

        // FILESYSTEM
        private string currentDirectory = @"0:\";

        // HISTORY
        private List<string> commandHistory = new List<string>();
        private int historyIndex = -1;

        // AUTOCOMPLETE
      

        private List<string> autoMatches = new List<string>();
        private int autoIndex = 0;

        private string helpFile = @"0:\help.txt";

        public WindowsConsole(int x, int y) : base("Console", x, y, 520, 420)
        {
            memManager = new ProcessMemoryManager("Console_System");
            buffer = new ConsoleBuffer(500);

            buffer.WriteLine("Console Ready.");
            buffer.WriteLine("Type 'help' for commands.");
        }

        // =========================
        // UPDATE
        // =========================
        public override void Update()
        {
            base.Update();
            if (IsMinimized) return;

            if (IsClosed)
            {
                memManager.ReleaseAll();
                return;
            }

            HandleMouse();
            HandleKeyboard();
        }

        // =========================
        // MOUSE (scroll + selection)
        // =========================
        private void HandleMouse()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            var state = MouseManager.MouseState;

            if (state == MouseState.Left)
            {
                // scroll drag
                if (IsInsideScrollBar(mx, my))
                {
                    draggingScroll = true;
                }

                // selection
                int line = GetLineFromMouse(my);

                if (selectionStart == -1)
                    selectionStart = line;

                selectionEnd = line;
            }
            else
            {
                draggingScroll = false;
            }

            if (draggingScroll)
            {
                int visible = (Height - 70) / lineHeight;
                var lines = buffer.GetLines();

                int total = lines.Length;
                int barHeight = Height - 80;

                int rel = my - (Y + 40);

                scrollOffset = (rel * total / Math.Max(1, barHeight)) - visible / 2;
                scrollOffset = Math.Max(0, Math.Min(scrollOffset, Math.Max(0, total - visible)));
            }
        }

        private bool IsInsideScrollBar(int mx, int my)
        {
            return mx >= X + Width - 12 &&
                   mx <= X + Width - 4 &&
                   my >= Y + 40 &&
                   my <= Y + Height - 40;
        }

        private int GetLineFromMouse(int my)
        {
            int start = Math.Max(0, scrollOffset);
            int index = (my - (Y + 40)) / lineHeight;
            return start + index;
        }

        // =========================
        // KEYBOARD
        // =========================
        private void HandleKeyboard()
        {
            if (!KeyboardManager.TryReadKey(out var key)) return;

            // COPY
            if (key.Key == ConsoleKeyEx.C && key.Modifiers == ConsoleModifiers.Control)
            {
                CopySelection();
                return;
            }

            // PASTE
            if (key.Key == ConsoleKeyEx.V && key.Modifiers == ConsoleModifiers.Control)
            {
                input = input.Insert(cursorIndex, clipboard);
                cursorIndex += clipboard.Length;
                return;
            }

            // SELECT ALL
            if (key.Key == ConsoleKeyEx.A && key.Modifiers == ConsoleModifiers.Control)
            {
                selectionStart = 0;
                selectionEnd = buffer.GetLines().Length;
                return;
            }

            // ENTER
            if (key.Key == ConsoleKeyEx.Enter)
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    commandHistory.Add(input);
                    historyIndex = commandHistory.Count;
                }

                ExecuteCommand(input.Trim());

                input = "";
                cursorIndex = 0;
                ResetAutocomplete();
                return;
            }

            // BACKSPACE
            if (key.Key == ConsoleKeyEx.Backspace)
            {
                if (cursorIndex > 0)
                {
                    input = input.Remove(cursorIndex - 1, 1);
                    cursorIndex--;
                }
                return;
            }

            // LEFT
            if (key.Key == ConsoleKeyEx.LeftArrow)
            {
                if (cursorIndex > 0) cursorIndex--;
                return;
            }

            // RIGHT
            if (key.Key == ConsoleKeyEx.RightArrow)
            {
                if (cursorIndex < input.Length) cursorIndex++;
                return;
            }

            // CHAR INPUT
            if (key.KeyChar != '\0')
            {
                input = input.Insert(cursorIndex, key.KeyChar.ToString());
                cursorIndex++;
                ResetAutocomplete();
            }
        }

        // =========================
        // COPY
        // =========================
        private void CopySelection()
        {
            if (selectionStart == -1 || selectionEnd == -1) return;

            var lines = buffer.GetLines();

            int s = Math.Min(selectionStart, selectionEnd);
            int e = Math.Max(selectionStart, selectionEnd);

            clipboard = "";

            for (int i = s; i <= e && i < lines.Length; i++)
                clipboard += lines[i] + "\n";
        }

        // =========================
        // COMMANDS
        // =========================
        private void ExecuteCommand(string cmd)
        {
            buffer.WriteLine(currentDirectory + "> " + cmd);
            var parts = cmd.Split(' ');

            try
            {
                switch (parts[0])
                {
                    case "help":
                        if (File.Exists(helpFile))
                        {
                            foreach (var l in File.ReadAllLines(helpFile))
                                buffer.WriteLine(l);
                        }
                        else
                        {
                            buffer.WriteLine("help.txt missing");
                        }
                        break;

                    case "mem":
                        buffer.WriteLine("RAM: " +
                            (Cosmos.Core.GCImplementation.GetUsedRAM() / 1024 / 1024) + " MB");
                        break;

                    case "clear":
                        buffer.Clear();
                        break;

                    case "ls":
                        foreach (var d in Directory.GetDirectories(currentDirectory))
                            buffer.WriteLine("[DIR] " + Path.GetFileName(d));

                        foreach (var f in Directory.GetFiles(currentDirectory))
                            buffer.WriteLine("      " + Path.GetFileName(f));
                        break;

                    case "cat":
                        if (parts.Length < 2) break;

                        string file = Path.Combine(currentDirectory, parts[1]);

                        if (File.Exists(file))
                            foreach (var l in File.ReadAllLines(file))
                                buffer.WriteLine(l);
                        break;

                    case "cd":
                        if (parts.Length < 2) break;

                        string target = parts[1] == ".."
                            ? Path.GetDirectoryName(currentDirectory.TrimEnd('\\')) + "\\"
                            : Path.Combine(currentDirectory, parts[1]) + "\\";

                        if (Directory.Exists(target))
                            currentDirectory = target;
                        break;

                    case "touch":
                        File.Create(Path.Combine(currentDirectory, parts[1])).Close();
                        break;

                    case "edit":
                        string path = Path.Combine(currentDirectory, parts[1]);
                        string content = cmd.Substring(cmd.IndexOf(parts[1]) + parts[1].Length).Trim();
                        File.WriteAllText(path, content);
                        break;

                    case "reboot":
                        Cosmos.System.Power.Reboot();
                        break;

                    case "shutdown":
                        Cosmos.System.Power.Shutdown();
                        break;

                    default:
                        buffer.WriteLine("Unknown command");
                        break;
                }
            }
            catch (Exception ex)
            {
                buffer.WriteLine("Error: " + ex.Message);
            }
        }

        // =========================
        // DRAW
        // =========================
        public override void Draw(Canvas canvas)
        {
            if (IsMinimized) return;

            base.Draw(canvas);

            int y = Y + 40;
            int visible = (Height - 70) / lineHeight;

            var lines = buffer.GetLines();

            int start = Math.Max(0, scrollOffset);
            int end = Math.Min(lines.Length, start + visible);

            for (int i = start; i < end; i++)
            {
                canvas.DrawString(lines[i],
                    PCScreenFont.Default,
                    StyleManager.TextLime,
                    X + 10,
                    y);

                y += lineHeight;
            }

            DrawScrollBar(canvas);

            DrawInput(canvas);
        }

        private void DrawInput(Canvas canvas)
        {
            string prompt = currentDirectory + "> ";

            int baseX = X + 10;
            int baseY = Y + Height - 25;

            canvas.DrawString(prompt, PCScreenFont.Default, StyleManager.TextWhite, baseX, baseY);

            int x = baseX + Measure(prompt);

            string before = input.Substring(0, cursorIndex);
            string after = input.Substring(cursorIndex);

            canvas.DrawString(before, PCScreenFont.Default, StyleManager.TextWhite, x, baseY);

            int cx = x + Measure(before);

            canvas.DrawFilledRectangle(StyleManager.TextWhite, cx, baseY, 2, lineHeight);

            canvas.DrawString(after, PCScreenFont.Default, StyleManager.TextWhite, cx + 2, baseY);
        }

        private void DrawScrollBar(Canvas canvas)
        {
            int barX = X + Width - 10;
            int barY = Y + 40;
            int barH = Height - 80;

            canvas.DrawFilledRectangle(StyleManager.DarkGray, barX, barY, 6, barH);

            int thumbH = 30;
            int thumbY = barY + scrollOffset;

            canvas.DrawFilledRectangle(StyleManager.LightGray, barX, thumbY, 6, thumbH);
        }

        // =========================
        // UTIL
        // =========================
        private int Measure(string text)
        {
            return text.Length * 8;
        }

        private void ResetAutocomplete()
        {
            autoMatches.Clear();
            autoIndex = 0;
        }
    }
}