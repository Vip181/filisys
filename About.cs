using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.GUI;
using filesys.System;
using System.Drawing;

namespace filesys
{
    internal unsafe class About : Window
    {
        // Buffers mémoire bas niveau (liés à cette fenêtre)
        private byte* textBuffer;

        private const uint TEXT_BUFFER_SIZE = 512;

        public About()
            : base("About", 200, 200, 400, 300)
        {
            // 🔥 Allocation mémoire dédiée à la fenêtre About
            textBuffer = OsMemoryManager.Alloc(Memory, TEXT_BUFFER_SIZE);

            // Exemple d’écriture bas niveau (optionnel)
            textBuffer[0] = (byte)'f';
            textBuffer[1] = (byte)'i';
            textBuffer[2] = (byte)'l';
            textBuffer[3] = (byte)'e';
            textBuffer[4] = (byte)'s';
            textBuffer[5] = (byte)'y';
            textBuffer[6] = (byte)'s';
        }

        public override void Draw(Canvas canvas)
        {
            if (canvas == null) return;

            base.Draw(canvas);

            // Texte affiché (haut niveau)
            canvas.DrawString(
                "filesys OS",
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 20,
                Y + 50
            );

            canvas.DrawString(
                "Version 1.0",
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 20,
                Y + 65
            );

            canvas.DrawString(
                "Developed by VINCENT SENUT",
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 20,
                Y + 80
            );

            canvas.DrawString(
                "2026",
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 20,
                Y + 110
            );
        }

        // ❌ Fermeture propre (libère toute la mémoire About)
        public override void Close()
        {
            base.Close();
        }
    }
}
