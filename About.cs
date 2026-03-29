using System;

namespace filesys.GUI
{
    public class About : BaseWindow
    {
        private System.ProcessMemoryManager memManager;
        private IntPtr imageBuffer;

        public About() : base("About", 150, 150, 300, 200)
        {
            memManager = new System.ProcessMemoryManager("AboutPage");

            // Exemple : On alloue de la mémoire pour un buffer de texte ou image
            imageBuffer = memManager.Allocate(1024);
        }

        public override void Update()
        {
            base.Update();

            // SI LA FENÊTRE EST FERMÉE : RESET TOTAL DE SA MÉMOIRE
            if (this.IsClosed)
            {
                memManager.ReleaseAll();
            }
        }
    }
}