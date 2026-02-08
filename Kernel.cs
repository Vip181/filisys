using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.GUI;
using filesys.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Sys = Cosmos.System;
namespace filesys
{
    public class Kernel : Sys.Kernel
    {
        // ================= SYSTEM =================
        Window aboutWindow;
        Window win;

        public static  Canvas canvass;
        // Crée ou met à jour la résolution par défaut
        protected override void BeforeRun()
        {
            OsMemoryManager.Init();


            filesys.CustomMouse.CurrentStyle = filesys.CursorConfig.Load();
 
            filesys.Config.initialefile();
            var (w, h) = Config.LoadResolution();
          
            // 2️⃣ Initialiser l’écran
            ScreenManager.Init(w, h);
            canvass = ScreenManager.Canvas;

            // ✅ CRÉATION UNIQUE DE LA CONSOLE
            aboutWindow = new About();
            win = new ConsoleWindow();
            canvass.Display();
        }

        protected override void Run()
        {
           

            // ✅ CLEAR À CHAQUE FRAME
            canvass.Clear(Color.FromArgb(30, 30, 30));

            // ✅ UPDATE AVANT DRAW
            win.Update();
            win.Draw(canvass);
            aboutWindow.Update();
            aboutWindow.Draw(canvass);
          
            // ✅ CURSEUR EN DERNIER
            filesys.CustomMouse.Draw(canvass);
            canvass.Display();
        }
    }
}
