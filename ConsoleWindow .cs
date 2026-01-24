using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Drawing;
using Sys = Cosmos.System;
namespace filesys
{
    class ConsoleWindow : Window
    {
        private string input = "";
        private string output = "";
        public bool HasFocus = true;

        public ConsoleWindow()
            : base("Console", 500, 700, 700,700)
        {
        }

        public override void Update()
        {
            base.Update();

            if (!HasFocus)
                return;

            if (KeyboardManager.TryReadKey(out var key))
            {
                if (key.Key == ConsoleKeyEx.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                }
                else if (key.Key == ConsoleKeyEx.Enter)
                {
                    output = ExecuteCommand(input);
                    input = "";
                }
                else if (key.KeyChar != '\0')
                {
                    input += key.KeyChar;
                }
            }
        }

        public override void Draw(Canvas canvas)
        {
            if (canvas == null) return;

            base.Draw(canvas);

            canvas.DrawString(
                "> " + input,
                PCScreenFont.Default,
                new Pen(Color.LightGreen),
                X + 10,
                Y + 40
            );

            canvas.DrawString(
                output,
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 10,
                Y + 60
            );
        }

        private string ExecuteCommand(string cmd)
        {
            if (cmd == "help")
                return "help, mem, clear, shutdown, restart, res Cursor block,  Cursor cross ,Cursor  arrow";

            if (cmd == "shutdown")
            {
                Power.Shutdown(); // ⚡ ARRÊT IMMÉDIAT
                return "";        // jamais vraiment affiché
            }
            if (cmd == "restart")
            {
                Power.Reboot(); // ⚡ ARRÊT IMMÉDIAT
                return "";        // jamais vraiment affiché
            }
            var parts = cmd.Split(' ');

         

            if (parts[0] == "cursor")
            {
                if (parts.Length == 2)
                {
                    switch (parts[1])
                    {
                        case "arrow":
                            filesys.CustomMouse.CurrentStyle = filesys.CursorStyle.Arrow;
                            return "Cursor -> arrow";

                        case "cross":
                            filesys.CustomMouse.CurrentStyle = filesys.CursorStyle.Cross;
                            return "Cursor -> cross";

                        case "block":
                            filesys.CustomMouse.CurrentStyle = filesys.CursorStyle.Block;
                            return "Cursor -> block";

                        default:
                            return "Styles disponibles: arrow, cross, block";
                    }
                }
                return "Format: cursor <style>";
            }


            if (cmd == "mem")
                return "RAM utilisée: " + MemoryManager.UsedMemory();

            if (cmd == "clear")
                return "";
            if (parts[0] == "res")
            {
                if (parts.Length == 3)
                {
                    int newWidth, newHeight;
                    if (int.TryParse(parts[1], out newWidth) &&
                        int.TryParse(parts[2], out newHeight))
                    {
                        ScreenManager.Init(newWidth, newHeight);
                        filesys.Kernel.canvass = ScreenManager.Canvas;
                        Config.SaveResolution(newWidth, newHeight);

                        return $"Resolution -> {newWidth}x{newHeight} (sauvegardée)";
                    }
                    return "Format: res <width> <height>";
                }
                return "Format: res <width> <height>";
            }

            return "Commande inconnue";
        }
    }
}
