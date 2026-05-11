using Cosmos.System.Graphics;
using filesys.System;
using System.Collections.Generic;
using System.IO;

namespace filesys.GUI
{
    public class DesktopManager
    {
        // ✅ Toutes les fenêtres passent ici
        public static WindowManager WindowMgr;

        // 📦 Icônes bureau
        public List<DesktopIcon> Icons =
            new List<DesktopIcon>();

        // 📁 Dossier courant
        private string currentPath = @"0:\";

        // ⬅ Historique navigation
        private Stack<string> history =
            new Stack<string>();

        // 🔄 Recharge les icônes
        public void Refresh()
        {
            Icons.Clear();

            // ⬅ BACK
            if (
                history.Count > 0 ||
                Directory.GetParent(currentPath) != null)
            {
                Icons.Add(
                    new DesktopIcon(
                        "Back",
                        "",
                        IconType.Back,
                        0,
                        0,
                        () => GoBack()
                    )
                );
            }

            // 📁 DOSSIERS
            foreach (
                string dir
                in FileSystemHelper.GetDirectories(currentPath))
            {
                string d = dir;

                string name =
                    Path.GetFileName(d);

                Icons.Add(
                    new DesktopIcon(
                        name,
                        d,
                        IconType.Folder,
                        0,
                        0,
                        () => ChangeDirectory(d)
                    )
                );
            }

            // 📄 FICHIERS
            foreach (
                string file
                in FileSystemHelper.GetFiles(currentPath))
            {
                string f = file;

                string name =
                    Path.GetFileName(f);

                Icons.Add(
                    new DesktopIcon(
                        name,
                        f,
                        IconType.File,
                        0,
                        0,
                        () => OpenFile(f)
                    )
                );
            }
        }

        // 📂 Changer dossier
        public void ChangeDirectory(
            string path,
            bool pushHistory = true)
        {
            try
            {
                if (
                    pushHistory &&
                    path != currentPath)
                {
                    history.Push(currentPath);
                }

                currentPath = path;

                Refresh();
            }
            catch
            {
            }
        }

        // ⬅ Retour
        public void GoBack()
        {
            try
            {
                if (history.Count > 0)
                {
                    currentPath =
                        history.Pop();

                    Refresh();
                    return;
                }

                DirectoryInfo parent =
                    Directory.GetParent(currentPath);

                if (parent != null)
                {
                    history.Push(currentPath);

                    currentPath =
                        parent.FullName;

                    Refresh();
                }
            }
            catch
            {
            }
        }

        // 📄 Ouvrir fichier
        public void OpenFile(string file)
        {
            try
            {
                string filePath = file;

                // 🔧 Corrige :
                // 0:\0:\...
                while (
                    filePath.StartsWith(@"0:\0:\"))
                {
                    filePath =
                        filePath.Substring(3);
                }

                // 🧠 Debug
                try
                {
                    File.WriteAllText(
                        @"0:\debug_last_opened.txt",
                        filePath
                    );
                }
                catch
                {
                }

                // ❌ WindowMgr absent
                if (WindowMgr == null)
                    return;

                // 📄 Viewer
                FileViewer viewer =
                    new FileViewer(
                        filePath,
                        200,
                        120
                    );

                // ✅ UNE SEULE LISTE
                WindowMgr.Add(viewer);
            }
            catch
            {
            }
        }

        // 🖱 Update icônes
        public void Update()
        {
            for (int i = 0; i < Icons.Count; i++)
            {
                if (Icons[i] != null)
                    Icons[i].Update();
            }
        }

        // 🎨 Draw icônes
        public void Draw(Canvas canvas)
        {
            if (canvas == null)
                return;

            int startX = 40;
            int startY = 40;

            int spacingX = 90;
            int spacingY = 90;

            int maxPerRow = 6;

            for (int i = 0; i < Icons.Count; i++)
            {
                if (Icons[i] == null)
                    continue;

                int col =
                    i % maxPerRow;

                int row =
                    i / maxPerRow;

                Icons[i].X =
                    startX + col * spacingX;

                Icons[i].Y =
                    startY + row * spacingY;

                Icons[i].Draw(canvas);
            }
        }
    }
}