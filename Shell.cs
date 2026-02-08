using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cosmos.HAL.PCIDevice;
using Sys = Cosmos.System;
namespace filesys
{
    class Shell
    {
        public static string Execute(string cmd)
        {
            string[] args = cmd.Split(' ');

            switch (args[0])
            {
                case "shutdown":
                    Sys.Power.Shutdown();
                    return "Arrêt du système...";
                    case "reboot":
                        Sys.Power.Reboot();
                        return "Redémarrage du système...";
                case "res":
                    if (args.Length == 3)
                    {
                        int newWidth, newHeight;
                        if (int.TryParse(args[1], out newWidth) && int.TryParse(args[2], out newHeight))
                        {
                            // 1️⃣ Mettre à jour l'écran
                            ScreenManager.Init(newWidth, newHeight);

                            // 2️⃣ Mettre à jour le canvas utilisé par le Kernel
                            // Attention : canvas est une variable globale dans Kernel
                           filesys.Kernel.canvass = ScreenManager.Canvas;

                            // 3️⃣ Sauvegarde dans le fichier
                           

                            return $"Resolution -> {newWidth}x{newHeight} (sauvegardée)";
                        }
                        return "Format: res <width> <height>";
                    }
                    return "Format: res <width> <height>";
                case "help":
                    return "help, mem, clear, echo";
                       
                case "echo":
                    return cmd.Substring(5);

                case "clear":
                    return "__CLEAR__";

                default:
                    return "Commande inconnue";
            }
        }
    }

}
