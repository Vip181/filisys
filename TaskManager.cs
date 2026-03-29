using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using filesys.System;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace filesys.GUI
{
    public class TaskManager : BaseWindow
    {
        public TaskManager(int x, int y) : base("Task Manager", x, y, 400, 350)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized) return;
            base.Draw(canvas);

            int yOffset = Y + 45;

            // --- SECTION : LISTE DES PROCESSUS ---
            canvas.DrawString("NOM DU PROCESSUS", PCScreenFont.Default, StyleManager.TextWhite, X + 10, yOffset);
            canvas.DrawString("ALLOC.", PCScreenFont.Default, StyleManager.TextWhite, X + 250, yOffset);

            yOffset += 20;
            canvas.DrawLine(StyleManager.TextWhite, X + 10, yOffset, X + Width - 10, yOffset);
            yOffset += 10;

            // Liste simulée (à lier à tes ProcessMemoryManager plus tard)
            DrawProcessLine(canvas, "Kernel System", 1, ref yOffset);
            DrawProcessLine(canvas, "Shell/Console", 8, ref yOffset);
            DrawProcessLine(canvas, "Desktop GUI", 4, ref yOffset);

            // --- SECTION : GRAPHIQUE DE RAM ---
            yOffset = Y + Height - 80;

            uint totalRamMB = Cosmos.Core.CPU.GetAmountOfRAM();
            uint usedRamMB = (uint)Cosmos.Core.GCImplementation.GetUsedRAM() / 1024 / 1024;

            // Calcul du pourcentage (0 à 100)
            float percentUsed = ((float)usedRamMB / (float)totalRamMB) * 100;
            int barWidth = Width - 40;
            int fillWidth = (int)((percentUsed / 100) * barWidth);

            // Dessin de la barre (Contour et fond vide)
            canvas.DrawString($"Utilisation RAM: {usedRamMB}MB / {totalRamMB}MB", PCScreenFont.Default, StyleManager.TextWhite, X + 20, yOffset - 20);

            // Fond de la barre (Gris foncé)
            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(40, 40, 40)), X + 20, yOffset, barWidth, 20);

            // Remplissage (Vert si < 70%, Orange si < 90%, Rouge sinon)
            Color barColor = Color.Lime;
            if (percentUsed > 70) barColor = Color.Orange;
            if (percentUsed > 90) barColor = Color.Red;

            canvas.DrawFilledRectangle(new Pen(barColor), X + 20, yOffset, fillWidth, 20);

            // Affichage du pourcentage au centre de la barre
            canvas.DrawString($"{(int)percentUsed}%", PCScreenFont.Default, new Pen(Color.Black), X + (Width / 2) - 10, yOffset + 2);
        }
        

        private void DrawProcessLine(Canvas canvas, string name, int allocs, ref int y)
        {
            canvas.DrawString(name, PCScreenFont.Default, StyleManager.TextLime, X + 15, y);
            canvas.DrawString(allocs.ToString(), PCScreenFont.Default, StyleManager.TextWhite, X + 260, y);
            y += 18;
        }
    }
}
   