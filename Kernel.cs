using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Sys = Cosmos.System;
using filesys.System; // Pour accéder à Config et autres utilitaires
using Cosmos.System.FileSystem.VFS;//
namespace filesys
{
    public class Kernel : Sys.Kernel
    {
        public static Kernel Instance;
        public static WindowManager WindowMgr;
        private Canvas canvas;

        public List<BaseWindow> windows = new List<BaseWindow>();

        // État et dimensions du menu (basé sur ton dessin)
        private bool menuOpen = false;
        private int menuWidth = 200;
        private int menuHeight = 350;

        protected override void BeforeRun()
        {
           

            Instance = this;
            // Initialisation de l'écran avec résolution sauvegardée
            var (w, h) = Config.LoadResolution();
            ScreenManager.Init(w, h);
            canvas = ScreenManager.Canvas;

            WindowMgr = new WindowManager();
            Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            // Ajouter une console au démarrage
            AddWindow(new WindowsConsole(100, 100));
        }

        protected override void Run()
        {
            // 1. Logique (Calculs, clics, etc.)
            UpdateSystem();

            // 2. Dessin
            canvas.Clear(Color.CornflowerBlue); // Fond d'écran

            // Dessiner les fenêtres
            WindowMgr.Draw(canvas);

            // Dessiner le menu s'il est ouvert
            if (menuOpen) DrawStartMenu();

            // Dessiner la barre des tâches (contient le bouton OS et les fenêtres)
            DrawTaskbar();

            // Dessiner la souris (toujours au premier plan)
            canvas.DrawFilledRectangle(new Pen(Color.White), (int)MouseManager.X, (int)MouseManager.Y, 8, 8);

            // 3. Rafraîchir l'écran
            canvas.Display();

            // Optimisation RAM
            Cosmos.Core.Memory.Heap.Collect();
        }

        public void AddWindow(BaseWindow w) => windows.Add(w);
        public List<BaseWindow> GetWindows() => windows;

        private void UpdateSystem()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool isClicked = MouseManager.MouseState == MouseState.Left;

            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;
            int menuY = taskbarY - menuHeight;

            if (isClicked)
            {
                // LOGIQUE CLIC DANS LE MENU OUVERT
                if (menuOpen && mx >= 0 && mx <= menuWidth && my >= menuY && my <= taskbarY)
                {
                    if (my > menuY + 20 && my < menuY + 80) { AddWindow(new WindowsConsole(150, 150)); menuOpen = false; }
                    else if (my > menuY + 100 && my < menuY + 160) { AddWindow(new TaskManager(150, 150)); menuOpen = false; }
                   

                    // Shutdown / Restart
                    else if (my > taskbarY - 40 && my < taskbarY)
                    {
                        if (mx < menuWidth / 2) Sys.Power.Shutdown();
                        else Sys.Power.Reboot();
                    }
                    Thread.Sleep(200);
                }
                // CLIC SUR LA BARRE DES TÂCHES
                else if (my >= taskbarY)
                {
                    // Bouton START
                    if (mx <= 50)
                    {
                        menuOpen = !menuOpen;
                        Thread.Sleep(200);
                    }
                    // Boutons des fenêtres
                    else
                    {
                        int tx = 55;
                        foreach (var win in windows)
                        {
                            if (mx >= tx && mx <= tx + 100)
                            {
                                win.IsMinimized = !win.IsMinimized;
                                menuOpen = false;
                                Thread.Sleep(150);
                                break;
                            }
                            tx += 110;
                        }
                    }
                }
                // CLIC SUR LE BUREAU (ferme le menu)
                else { menuOpen = false; }
            }

            // LOGIQUE DE MISE À JOUR DES FENÊTRES (Drag & Drop, etc.)
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                var win = windows[i];
                if (!win.IsMinimized) win.Update();
                if (win.IsClosed) windows.RemoveAt(i);
            }
        }

        private void DrawStartMenu()
        {
            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;
            int menuY = taskbarY - menuHeight;

            // Fond du menu
            canvas.DrawFilledRectangle(new Pen(Color.Gray), 0, menuY, menuWidth, menuHeight);
            canvas.DrawRectangle(new Pen(Color.Black), 0, menuY, menuWidth, menuHeight);

            // Icônes dessinées à la main
            DrawConsoleIcon(10, menuY + 20);
            canvas.DrawString("console", PCScreenFont.Default, new Pen(Color.Black), 60, menuY + 35);

            DrawTasksIcon(10, menuY + 100);
            canvas.DrawString("tasks", PCScreenFont.Default, new Pen(Color.Black), 60, menuY + 115);

            DrawOptionIcon(10, menuY + 180);
            canvas.DrawString("option", PCScreenFont.Default, new Pen(Color.Black), 60, menuY + 195);

            // Boutons Power (Bleu)
            int btnWidth = menuWidth / 2;
            canvas.DrawFilledRectangle(new Pen(Color.RoyalBlue), 0, taskbarY - 40, btnWidth, 40);
            canvas.DrawFilledRectangle(new Pen(Color.RoyalBlue), btnWidth, taskbarY - 40, btnWidth, 40);
            canvas.DrawRectangle(new Pen(Color.Black), 0, taskbarY - 40, menuWidth, 40);
            canvas.DrawLine(new Pen(Color.Black), btnWidth, taskbarY - 40, btnWidth, taskbarY);

            canvas.DrawString("shutdown", PCScreenFont.Default, new Pen(Color.White), 10, taskbarY - 25);
            canvas.DrawString("restart", PCScreenFont.Default, new Pen(Color.White), btnWidth + 15, taskbarY - 25);
        }

        private void DrawTaskbar()
        {
            int screenWidth = (int)canvas.Mode.Columns;
            int screenHeight = (int)canvas.Mode.Rows;
            int taskbarY = screenHeight - 30;

            // Fond barre des tâches
            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(20, 20, 20)), 0, taskbarY, screenWidth, 30);

            // Bouton START "OS"
            canvas.DrawFilledRectangle(new Pen(Color.White), 5, taskbarY + 5, 40, 20);
            canvas.DrawString("OS", PCScreenFont.Default, new Pen(Color.Black), 15, taskbarY + 8);

            // DESSIN DES BOUTONS DE FENÊTRES (Tes boutons manquants)
            int tx = 55;
            foreach (var win in windows)
            {
                Color btnColor = win.IsMinimized ? Color.FromArgb(60, 60, 60) : Color.FromArgb(100, 100, 100);
                canvas.DrawFilledRectangle(new Pen(btnColor), tx, taskbarY + 4, 100, 22);

                string label = win.Title.Length > 8 ? win.Title.Substring(0, 7) + ".." : win.Title;
                canvas.DrawString(label, PCScreenFont.Default, new Pen(Color.White), tx + 5, taskbarY + 7);

                tx += 110;
            }
        }

        // --- TES ICÔNES À PERSONNALISER ---
        private void DrawConsoleIcon(int x, int y)
        {
            canvas.DrawRectangle(new Pen(Color.Black), x, y, 40, 30);
            canvas.DrawLine(new Pen(Color.Black), x + 10, y + 30, x + 5, y + 35);
            canvas.DrawLine(new Pen(Color.Black), x + 30, y + 30, x + 35, y + 35);
        }

        private void DrawTasksIcon(int x, int y)
        {
            canvas.DrawFilledRectangle(new Pen(Color.Black), x + 10, y + 10, 20, 20);
        }

        private void DrawOptionIcon(int x, int y)
        {
            canvas.DrawLine(new Pen(Color.Black), x, y, x + 30, y + 30);
            canvas.DrawLine(new Pen(Color.Black), x + 30, y, x, y + 30);
        }
    }
}