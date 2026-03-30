using System;
using System.Collections.Generic;
using Cosmos.System.Graphics;
using System.Drawing;
using filesys.System;
using Cosmos.System.Graphics.Fonts;

namespace filesys.GUI
{
    public class TaskManager : BaseWindow
    {
        // On définit des couleurs réutilisables pour le style
        private Color colorGraphBg = Color.FromArgb(40, 40, 40);
        private Color colorRowAlt = Color.FromArgb(35, 35, 35);

        public TaskManager(int x, int y) : base("Task Manager", x, y, 400, 350)
        {
            // Taille par défaut fixée à 400x350
        }

        public override void Draw(Canvas canvas)
        {
            if (IsMinimized) return;

            // 1. Dessiner la base de la fenêtre (Titre, bordures, fond)
            base.Draw(canvas);

            int yOffset = Y + 45;

            // --- SECTION : EN-TÊTE ---
            canvas.DrawString("PROCESSUS", PCScreenFont.Default, StyleManager.TextWhite, X + 15, yOffset);
            canvas.DrawString("ETAT", PCScreenFont.Default, StyleManager.TextWhite, X + 200, yOffset);
            canvas.DrawString("ID", PCScreenFont.Default, StyleManager.TextWhite, X + 330, yOffset);

            yOffset += 20;
            canvas.DrawLine(StyleManager.TextWhite, X + 10, yOffset, X + Width - 10, yOffset);
            yOffset += 10;

            // --- SECTION : LISTE RÉELLE DES FENÊTRES ---
            // On récupère la liste des fenêtres directement depuis le Kernel
            var windows = Kernel.Instance.GetWindows();

            for (int i = 0; i < windows.Count; i++)
            {
                var win = windows[i];

                // Alternance de couleur pour les lignes (plus lisible)
                if (i % 2 == 0)
                {
                    canvas.DrawFilledRectangle(new Pen(colorRowAlt), X + 10, yOffset - 2, Width - 20, 18);
                }

                string status = win.IsMinimized ? "Reduit" : "Actif";

                // Dessin d'une ligne de processus
                canvas.DrawString(win.Title, PCScreenFont.Default, StyleManager.TextLime, X + 15, yOffset);
                canvas.DrawString(status, PCScreenFont.Default, StyleManager.TextWhite, X + 200, yOffset);
                canvas.DrawString("#" + i, PCScreenFont.Default, new Pen(Color.Gray), X + 330, yOffset);

                yOffset += 20;

                // Sécurité pour ne pas dépasser de la fenêtre si trop de processus
                if (yOffset > Y + Height - 100) break;
            }

            // --- SECTION : GRAPHIQUE DE LA MÉMOIRE RAM ---
            DrawMemoryGraph(canvas);
        }

        private void DrawMemoryGraph(Canvas canvas)
        {
            int graphY = Y + Height - 80;
            int margin = 20;
            int barWidth = Width - (margin * 2);
            int barHeight = 22;

            uint totalRamMB = Cosmos.Core.CPU.GetAmountOfRAM();
            uint usedRamMB = (uint)Cosmos.Core.GCImplementation.GetUsedRAM() / 1024 / 1024;

            float usageRatio = (float)usedRamMB / (float)totalRamMB;
            if (usageRatio > 1.0f) usageRatio = 1.0f;

            int fillWidth = (int)(usageRatio * barWidth);
            int percent = (int)(usageRatio * 100);

            canvas.DrawString($"Memoire : {usedRamMB} MB / {totalRamMB} MB", PCScreenFont.Default, StyleManager.TextWhite, X + margin, graphY - 20);

            // Correction ici : Color -> Pen
            canvas.DrawFilledRectangle(new Pen(colorGraphBg), X + margin, graphY, barWidth, barHeight);

            Color colorBar = Color.Lime;
            if (percent > 70) colorBar = Color.Orange;
            if (percent > 90) colorBar = Color.Red;

            if (fillWidth > 0)
            {
                // Correction ici : Color -> Pen
                canvas.DrawFilledRectangle(new Pen(colorBar), X + margin, graphY, fillWidth, barHeight);
            }

            canvas.DrawString($"{percent}%", PCScreenFont.Default, new Pen(Color.Black), X + (Width / 2) - 10, graphY + 4);
        }
    }
}