using System.Drawing;
using Cosmos.System.Graphics;
namespace filesys.System
{
    public struct RenderCommand
    {
        public string Text;
        public int X;
        public int Y;
        public Pen Pen;

        public RenderCommand(string text, int x, int y, Pen pen)
        {
            Text = text;
            X = x;
            Y = y;
            Pen = pen;
        }
    }
}
