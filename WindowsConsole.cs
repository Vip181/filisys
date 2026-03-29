using System;
using System.IO;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.System; // Pour accéder à ProcessMemoryManager
using System.Drawing;

namespace filesys.GUI
{
    public class WindowsConsole : BaseWindow
    {
        private ConsoleBuffer buffer;
        private string input = "";
        private int lineHeight = 16;

        // Notre gestionnaire de mémoire dédié à cette instance de console
        private ProcessMemoryManager memManager;

        public WindowsConsole(int x, int y) : base("Console", x, y, 520, 420)
        {
            // Initialisation du gestionnaire de mémoire
            memManager = new ProcessMemoryManager("Console_System");

            // On alloue le buffer de la console via le manager
            // On considère que le ConsoleBuffer est un objet "suivi"
            buffer = new ConsoleBuffer(100);

            buffer.WriteLine("Console Ready.");
            buffer.WriteLine("Memory Manager: ACTIVE");
        }

        public void ShowError(string message)
        {
            buffer.WriteLine("Error: " + message);
        }

        public override void Update()
        {
            base.Update();
            if (IsMinimized) return;

            // SI LA CONSOLE EST FERMÉE (clic sur le X)
            if (this.IsClosed)
            {
                // RESET TOTAL : On libère tous les pointeurs alloués par cette console
                memManager.ReleaseAll();
                return;
            }

            if (KeyboardManager.TryReadKey(out var key))
            {
                if (key.Key == ConsoleKeyEx.Enter)
                {
                    ExecuteCommand(input.ToLower().Trim());
                    input = "";
                }
                else if (key.Key == ConsoleKeyEx.Backspace && input.Length > 0)
                    input = input.Remove(input.Length - 1);
                else if (key.KeyChar != '\0' && input.Length < 60)
                    input += key.KeyChar;
            }
        }

        private void ExecuteCommand(string cmd)
        {
            buffer.WriteLine("> " + cmd);
            var parts = cmd.Split(' ');

            try
            {
                if (cmd == "mem")
                {
                    uint usedRam = (uint)Cosmos.Core.GCImplementation.GetUsedRAM() / 1024 / 1024;
                    buffer.WriteLine($"RAM Total: {Cosmos.Core.CPU.GetAmountOfRAM()} MB");
                    buffer.WriteLine($"Allocations Process: {memManager.GetAllocationCount()}");
                }
                else if (cmd == "ramfix")
                {
                    // On nettoie la mémoire globale
                    Cosmos.Core.Memory.Heap.Collect();
                    buffer.WriteLine("System Memory Reset.");
                }
                else if (cmd == "reboot")
                {
                    buffer.WriteLine("Rebooting...");
                    Cosmos.System.Power.Reboot();
                }
                else if (cmd == "shutdown")
                {
                    buffer.WriteLine("Shutting down...");
                    Cosmos.System.Power.Shutdown();
                }
                else if (cmd == "clear")
                {
                    buffer.Clear();
                }
                else if (cmd == "tasks")
                {
                    // On demande au Kernel d'ajouter une nouvelle fenêtre de TaskManager
                    // Note: Il faut que ta liste 'windows' dans Kernel soit accessible ou via une méthode statique
                    Kernel.Instance.AddWindow(new TaskManager(200, 200));
                    buffer.WriteLine("Lancement du Gestionnaire de taches...");
                }
                else if (cmd == "help")
                {
                    buffer.WriteLine("Cmds: mem, ramfix, clear, res, reboot, shutdown,tasks");
                }
                else if (parts[0] == "res" && parts.Length == 3)
                {
                    if (int.TryParse(parts[1], out int w) && int.TryParse(parts[2], out int h))
                    {
                        buffer.WriteLine($"Switching to {w}x{h}...");
                        Config.SaveResolution(w, h);
                        // Le Kernel s'occupera du changement au reboot
                    }
                }
                else
                {
                    buffer.WriteLine("Unknown command");
                }
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized) return;
            base.Draw(canvas);

            // Rendu adaptatif du texte
            int availableHeight = Height - 70;
            int maxLines = availableHeight / lineHeight;
            var lines = buffer.GetLines();
            int yOffset = Y + 40;

            int start = Math.Max(0, lines.Length - maxLines);

            for (int i = start; i < lines.Length; i++)
            {
                if (yOffset + lineHeight < Y + Height - 25)
                {
                    canvas.DrawString(lines[i], PCScreenFont.Default, StyleManager.TextLime, X + 10, yOffset);
                    yOffset += lineHeight;
                }
            }

            canvas.DrawString("> " + input + "_", PCScreenFont.Default, StyleManager.TextWhite, X + 10, Y + Height - 25);
        }
    }
}