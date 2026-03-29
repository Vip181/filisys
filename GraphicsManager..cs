using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System.Collections.Generic;
using System.Drawing;

namespace filesys.System
{
    public static class GraphicsManager
    {
        public static Canvas Canvas;

        // Police par défaut
        public static readonly PCScreenFont Font = PCScreenFont.Default;

        // Pens réutilisables (évite allocations)
        public static readonly Pen WhitePen = new Pen(Color.White);
        public static readonly Pen GreenPen = new Pen(Color.LightGreen);
        public static readonly Pen RedPen = new Pen(Color.Red);
        public static readonly Pen YellowPen = new Pen(Color.Yellow);
        public static readonly Pen GrayPen = new Pen(Color.Gray);
        public static readonly Pen DarkGrayPen = new Pen(Color.FromArgb(50, 50, 50));
            public static readonly Pen DarkGrayBrush = new Pen(Color.DarkGray);
            public static readonly Pen GrayBrush = new Pen(Color.Gray);
        // 🔥 Liste des commandes batchées
        private static List<RenderCommand> commands =
            new List<RenderCommand>(512);

        // =========================
        // INIT
        // =========================
        public static void Init(Canvas canvas)
        {
            Canvas = canvas;
        }

        // =========================
        // CLEAR
        // =========================
        public static void Clear()
        {
            if (Canvas == null)
                return;

            Canvas.Clear(Color.FromArgb(30, 30, 30));
        }

        // =========================
        // AJOUT AU BATCH
        // =========================
        public static void DrawString(string text, int x, int y, Pen pen)
        {
            if (Canvas == null || text == null)
                return;

            commands.Add(new RenderCommand(text, x, y, pen));
        }

        // =========================
        // 🔥 RENDER FINAL
        // =========================
        public static void RenderAll()
        {
            if (Canvas == null)
                return;

            for (int i = 0; i < commands.Count; i++)
            {
                var cmd = commands[i];

                Canvas.DrawString(
                    cmd.Text,
                    Font,
                    cmd.Pen,
                    cmd.X,
                    cmd.Y
                );
            }

            Canvas.Display();

            commands.Clear();
        }
    }
}
