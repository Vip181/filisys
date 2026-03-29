using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Sys = Cosmos.System;

namespace filesys
{
    public class Kernel : Sys.Kernel
    {
        public static Canvas canvas;
        private List<BaseWindow> windows = new List<BaseWindow>();

        public static Kernel Instance;

        protected override void BeforeRun()
        {
            Instance = this;

            try
            {
                // Initialisation du système de fichiers
                Config.initialefile();
            }
            catch
            {
                // Optionnel : logger l'erreur si le FS échoue
            }
            var (w, h) = Config.LoadResolution();

            // 2️⃣ Initialiser l’écran
            ScreenManager.Init(w, h);
            // Initialisation graphique (Auto-adaptatif via Config ou fixe)
            // Utilisation de GetFullScreenCanvas pour le mode natif
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(w, h, ColorDepth.ColorDepth32));

            // Chargement des fenêtres par défaut
            windows.Add(new WindowsConsole(50, 50));
            windows.Add(new About());
        }
        public void AddWindow(BaseWindow win)
        {
            windows.Add(win);
        }
        protected override void Run()
        {
            try
            {
                // 1. LOGIQUE SYSTÈME (Calculs et entrées)
                UpdateSystem();

                // 2. RENDU (Dessin sur le buffer)
                // Fond d'écran
                canvas.Clear(Color.SlateGray);
                filesys.CustomMouse.Draw(canvas);
                // Dessin des fenêtres (dans l'ordre de la liste)
                foreach (var win in windows)
                {
                    win.Draw(canvas);
                }

                // Dessin de la barre des tâches (toujours par-dessus les fenêtres)
                DrawTaskbar();

                // Dessin du curseur (Carré blanc 4x4)
                // Il est dessiné en dernier pour être toujours visible
                canvas.DrawFilledRectangle(new Pen(Color.White), (int)MouseManager.X, (int)MouseManager.Y, 4, 4);

                // 3. AFFICHAGE FINAL
                canvas.Display();
            }
            catch (Exception ex)
            {
                // Gestion rudimentaire des crashs (Panic)
                foreach (var win in windows)
                {
                    if (win is WindowsConsole console)
                    {
                        console.ShowError("KERNEL ERROR: " + ex.Message);
                    }
                }
            }
        }

        private void UpdateSystem()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool isClicked = MouseManager.MouseState == MouseState.Left;
            BaseWindow windowToFocus = null;

            // --- GESTION DES FENÊTRES ---
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                var win = windows[i];

                if (isClicked && !win.IsMinimized)
                {
                    if (mx >= win.X && mx <= win.X + win.Width && my >= win.Y && my <= win.Y + win.Height)
                    {
                        if (windowToFocus == null) windowToFocus = win;
                    }
                }

                win.Update();

                if (win.IsClosed)
                {
                    windows.RemoveAt(i);
                }
            }

            if (windowToFocus != null)
            {
                windows.Remove(windowToFocus);
                windows.Add(windowToFocus);
            }

            // --- GESTION BARRE DES TÂCHES DYNAMIQUE ---
            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;

            if (isClicked && my >= taskbarY)
            {
                int tx = 10;
                foreach (var win in windows)
                {
                    if (mx >= tx && mx <= tx + 100)
                    {
                        win.IsMinimized = !win.IsMinimized;
                        Thread.Sleep(100);
                    }
                    tx += 110;
                }
            }
        }

        private void DrawTaskbar()
        {
            int screenWidth = (int)canvas.Mode.Columns;
            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;

            // 1. Fond de la barre (S'adapte à la largeur de l'écran)
            canvas.DrawFilledRectangle(StyleManager.TaskbarBg, 0, taskbarY, screenWidth, 30);

            // 2. Boutons des fenêtres
            int tx = 10;
            foreach (var win in windows)
            {
                canvas.DrawFilledRectangle(StyleManager.TitleBar, tx, taskbarY + 4, 100, 22);

                string label = win.Title.Length > 8 ? win.Title.Substring(0, 7) + ".." : win.Title;
                canvas.DrawString(label, PCScreenFont.Default, StyleManager.TextWhite, tx + 5, taskbarY + 7);

                tx += 110;
            }
        }

        public void ResetScreen(int w, int h)
        {
            // Sauvegarde la nouvelle résolution dans le fichier screen.txt via Config
            Config.SaveResolution(w, h);
        }
    }
}