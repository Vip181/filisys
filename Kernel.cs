using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using Sys = Cosmos.System;
using System.IO;
namespace filesys
{
    public class Kernel : Sys.Kernel
    {
        // ================= SYSTEM =================
        Window consoleWindow;
     public static  Canvas canvass;

       

        // Crée ou met à jour la résolution par défaut
       



        protected override void BeforeRun()
        {

           System.Console.WriteLine("BOOT START");
          





         
            System.Console.WriteLine("VFS OK");
           
            filesys.CustomMouse.CurrentStyle = filesys.CursorConfig.Load();
           System.Console.WriteLine("CURSOR OK");
            filesys.Config.initialefile();
            var (w, h) = Config.LoadResolution();
          
            // 2️⃣ Initialiser l’écran
            ScreenManager.Init(w, h);
            canvass = ScreenManager.Canvas;

            // ✅ CRÉATION UNIQUE DE LA CONSOLE
            consoleWindow = new ConsoleWindow();

          System.Console.WriteLine("GRAPHICS OK");
        }

        protected override void Run()
        {


            // ✅ CLEAR À CHAQUE FRAME
            canvass.Clear(Color.FromArgb(30, 30, 30));

            // ✅ UPDATE AVANT DRAW
            consoleWindow.Update();
            consoleWindow.Draw(canvass);

            // ✅ CURSEUR EN DERNIER
            filesys.CustomMouse.Draw(canvass);

            // INFO RAM
                canvass.DrawString(
                "RAM utilisée: " + MemoryManager.UsedMemory() + " bytes",
                PCScreenFont.Default,
                new Pen(Color.White), // Correction ici : utilisation d'un Pen
                10,
                740
            );

            canvass.Display();
        }
    }
}
