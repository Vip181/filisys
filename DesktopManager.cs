using Cosmos.System.Graphics;
using filesys.System;
using System.Collections.Generic;
using System.IO;

namespace filesys.GUI
{
    public class DesktopManager
    {
        public List<DesktopIcon> Icons = new List<DesktopIcon>();

        private string currentPath = @"0:\";
        private Stack<string> history = new Stack<string>();

        public void Refresh()
        {
            Icons.Clear();

            // Icône de retour si on a un historique ou un parent
            if (history.Count > 0 || Directory.GetParent(currentPath) != null)
            {
                // icône "Back" : retourne au répertoire précédent (historique) ou au parent
                Icons.Add(new DesktopIcon(
                    "Back",
                    "", // pas de path associé
                    IconType.Back,
                    0, 0,
                    () => GoBack()
                ));
            }

            // 📁 dossiers
            foreach (var dir in FileSystemHelper.GetDirectories(currentPath))
            {
                var d = dir; // capturer la valeur locale pour la lambda
                string name = Path.GetFileName(d);

                Icons.Add(new DesktopIcon(
                    name,
                    d,
                    IconType.Folder,
                    0, 0,
                    () => ChangeDirectory(d)
                ));
            }

            // 📄 fichiers
            foreach (var file in FileSystemHelper.GetFiles(currentPath))
            {
                var f = file; // capturer la valeur locale pour la lambda
                string name = Path.GetFileName(f);

                Icons.Add(new DesktopIcon(
                    name,
                    f,
                    IconType.File,
                    0, 0,
                    () => OpenFile(f)
                ));
            }
        }

        // pushHistory true par défaut : empile l'ancien répertoire dans l'historique
        public void ChangeDirectory(string path, bool pushHistory = true)
        {
            if (pushHistory && path != currentPath)
            {
                history.Push(currentPath);
            }

            currentPath = path;
            Refresh();
        }

        public void GoBack()
        {
            if (history.Count > 0)
            {
                var prev = history.Pop();
                // ne pas empiler l'actuel quand on retourne
                currentPath = prev;
                Refresh();
                return;
            }

            // si pas d'historique, remonter d'un niveau si possible
            var parent = Directory.GetParent(currentPath);
            if (parent != null)
            {
                // on peut empiler l'actuel pour permettre un retour
                history.Push(currentPath);
                currentPath = parent.FullName;
                Refresh();
            }
        }

        public static WindowManager WindowMgr;

        public void OpenFile(string file)
        {
            // normaliser le chemin pour éviter doublons tels que "0:\0:\..."
            var filePath = file;
            while (filePath.StartsWith(@"0:\0:\"))
                filePath = filePath.Substring(3); // supprime le premier "0:\")

            // trace pour debug : écrire le chemin ouvert (si possible)
            try
            {
                File.WriteAllText(@"0:\debug_last_opened.txt", filePath);
            }
            catch { /* ignore si impossible d'écrire */ }

            // Sécurité : s'assurer que le Kernel est prêt
            if (Kernel.Instance == null)
                return;

            FileViewer viewer = new FileViewer(filePath, 200, 120);
            Kernel.Instance.AddWindow(viewer);
        }

        public void Update()
        {
            foreach (var icon in Icons)
                icon.Update();
        }

        public void Draw(Canvas canvas)
        {
            int startX = 40;
            int startY = 40;
            int spacingX = 90;
            int spacingY = 90;
            int maxPerRow = 6;

            for (int i = 0; i < Icons.Count; i++)
            {
                int col = i % maxPerRow;
                int row = i / maxPerRow;

                Icons[i].X = startX + col * spacingX;
                Icons[i].Y = startY + row * spacingY;

                Icons[i].Draw(canvas);
            }
        }
    }
}