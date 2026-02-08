using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.Commands;
using filesys.GUI;
using filesys.Input;
using filesys.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace filesys
{
    public unsafe class ConsoleWindow : Window
    {
        // ===== ÉTAT CONSOLE =====
        private string input = "";
        private string output = "";
        public bool HasFocus = true;

        // ===== BUFFERS MÉMOIRE =====
        private byte* inputBuffer;
        private byte* outputBuffer;

        private const uint INPUT_BUFFER_SIZE = 512;
        private const uint OUTPUT_BUFFER_SIZE = 2048;

        // ===== HISTORIQUE =====
        private List<string> history = new List<string>();
        private int historyIndex = -1;

        // ===== AUTO-RESIZE =====
        private const int LINE_HEIGHT = 16;   // PCScreenFont
        private const int TOP_PADDING = 60;   // barre titre + marges
        private const int BOTTOM_PADDING = 20;
        private const int MIN_HEIGHT = 150;

        public ConsoleWindow()
            : base("Console", 100, 100, 700, MIN_HEIGHT)
        {
            inputBuffer = OsMemoryManager.Alloc(Memory, INPUT_BUFFER_SIZE);
            outputBuffer = OsMemoryManager.Alloc(Memory, OUTPUT_BUFFER_SIZE);
        }

        // =========================
        // UPDATE
        // =========================
        public override void Update()
        {
            base.Update();
            if (!HasFocus) return;

            if (KeyboardManager.TryReadKey(out var key))
            {
                // 🔒 CAPS LOCK
                if (key.Key == ConsoleKeyEx.CapsLock)
                {
                    KeyboardLayoutFR.CapsLockEnabled =
                        !KeyboardLayoutFR.CapsLockEnabled;
                    return;
                }

                // 🔙 BACKSPACE
                if (key.Key == ConsoleKeyEx.Backspace)
                {
                    if (input.Length > 0)
                        input = input.Substring(0, input.Length - 1);
                    return;
                }

                // ⏎ ENTER
                if (key.Key == ConsoleKeyEx.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(input))
                        history.Add(input);

                    historyIndex = -1;
                    output = ExecuteCommand(input);
                    input = "";

                    AutoResize();
                    return;
                }

                // ⬆ HISTORIQUE
                if (key.Key == ConsoleKeyEx.UpArrow && history.Count > 0)
                {
                    if (historyIndex < 0)
                        historyIndex = history.Count - 1;
                    else if (historyIndex > 0)
                        historyIndex--;

                    input = history[historyIndex];
                    return;
                }

                // ⬇ HISTORIQUE
                if (key.Key == ConsoleKeyEx.DownArrow)
                {
                    if (historyIndex >= 0 && historyIndex < history.Count - 1)
                    {
                        historyIndex++;
                        input = history[historyIndex];
                    }
                    else
                    {
                        historyIndex = -1;
                        input = "";
                    }
                    return;
                }

                // ✍️ TEXTE (AZERTY + SHIFT + CAPS)
                if (key.KeyChar != '\0')
                {
                    bool shift =
                        (key.Modifiers & ConsoleModifiers.Shift) != 0;

                    char c = KeyboardLayoutFR.Convert(
                        key.Key,
                        key.KeyChar,
                        shift
                    );

                    if (c != '\0')
                        input += c;
                }
            }
        }

        // =========================
        // DRAW
        // =========================
        public override void Draw(Canvas canvas)
        {
            if (canvas == null) return;

            base.Draw(canvas);

            // Ligne de commande
            canvas.DrawString(
                "> " + input,
                PCScreenFont.Default,
                new Pen(Color.LightGreen),
                X + 10,
                Y + 40
            );

            // Sortie
            canvas.DrawString(
                output,
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 10,
                Y + 60
            );

            // Indicateur CAPS LOCK
            if (KeyboardLayoutFR.CapsLockEnabled)
            {
                canvas.DrawString(
                    "CAPS",
                    PCScreenFont.Default,
                    new Pen(Color.Red),
                    X + Width - 60,
                    Y + 5
                );
            }

            AutoResize();
        }

        // =========================
        // AUTO-RESIZE
        // =========================
        private void AutoResize()
        {
            int lines = 1;

            if (!string.IsNullOrEmpty(output))
                lines += output.Split('\n').Length;

            // +1 ligne pour l'input
            lines += 1;

            int neededHeight =
                TOP_PADDING +
                (lines * LINE_HEIGHT) +
                BOTTOM_PADDING;

            if (neededHeight < MIN_HEIGHT)
                neededHeight = MIN_HEIGHT;

            Height = neededHeight;

            // Sécurité écran
            if (Y + Height > ScreenManager.Height)
                Height = ScreenManager.Height - Y;
        }

        // =========================
        // COMMANDES
        // =========================
        private string ExecuteCommand(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
                return "";

            if (cmd == "help")
            {
                return
                    "help, mem, clear, shutdown, restart\n" +
                    "cursor <arrow|cross|block>\n" +
                    "res <width> <height>\n" +
                    "ping <ip|hostname>\n" +
                    "cat <file>";
            }

            if (cmd == "shutdown")
            {
                Power.Shutdown();
                return "";
            }

            if (cmd == "restart")
            {
                Power.Reboot();
                return "";
            }

            if (cmd == "clear")
            {
                output = "";
                return "";
            }

            if (cmd == "mem")
            {
                return OsMemoryManager.GetStatus();
            }

            var parts = cmd.Split(' ');

            // ===== CURSOR =====
            if (parts[0] == "cursor")
            {
                if (parts.Length == 2)
                {
                    switch (parts[1])
                    {
                        case "arrow":
                            CustomMouse.CurrentStyle = CursorStyle.Arrow;
                            return "Cursor -> arrow";

                        case "cross":
                            CustomMouse.CurrentStyle = CursorStyle.Cross;
                            return "Cursor -> cross";

                        case "block":
                            CustomMouse.CurrentStyle = CursorStyle.Block;
                            return "Cursor -> block";

                        default:
                            return "Styles: arrow, cross, block";
                    }
                }
                return "Format: cursor <style>";
            }

            // ===== RÉSOLUTION =====
            if (parts[0] == "res")
            {
                if (parts.Length == 3)
                {
                    if (int.TryParse(parts[1], out int w) &&
                        int.TryParse(parts[2], out int h))
                    {
                        ScreenManager.Init(w, h);
                        Kernel.canvass = ScreenManager.Canvas;
                        Config.SaveResolution(w, h);

                        return $"Resolution -> {w}x{h} (sauvegardée)";
                    }
                }
                return "Format: res <width> <height>";
            }

            // ===== PING =====
            if (parts[0] == "ping")
            {
                if (parts.Length == 2)
                    return PingCommand.Execute(parts[1]);

                return "Format: ping <ip | hostname>";
            }

            // ===== CAT =====
            if (parts[0] == "cat")
            {
                if (parts.Length == 2)
                {
                    try
                    {
                        if (!File.Exists(parts[1]))
                            return "Fichier introuvable";

                        return File.ReadAllText(parts[1]);
                    }
                    catch (Exception e)
                    {
                        return "Erreur lecture fichier: " + e.Message;
                    }
                }
                return "Format: cat <fichier>";
            }

            return "Commande inconnue";
        }

        // =========================
        // FERMETURE PROPRE
        // =========================
        public override void Close()
        {
            base.Close();
        }
    }
}
