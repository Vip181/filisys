using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
namespace filesys
{
    internal class About : Window

    {
        public About()
            : base("About", 200, 200, 400, 300)
        {
        }
        public override void Draw(Canvas canvas)
        {
            if (canvas == null) return;
            base.Draw(canvas);
            string aboutText = "filesys OS";
            string abouttextligne1 = "Developed by VINCENT SENUT ";
             string abouttextligne3 = "2024";
            string aboutTextligne2 = "Version 1.0";


            canvas.DrawString(
                aboutText,
                PCScreenFont.Default,
                new Pen(Color.White),
                X + 20,
                Y + 50
            );
            canvas.DrawString(
              aboutTextligne2,
              PCScreenFont.Default,
              new Pen(Color.White),
              X + 20,
              Y + 65
          );
            canvas.DrawString(
              abouttextligne1,
              PCScreenFont.Default,
              new Pen(Color.White),
              X + 20,
              Y + 80
          );
            canvas.DrawString(
             abouttextligne3,
             PCScreenFont.Default,
             new Pen(Color.White),
             X + 20,
             Y + 110);

        }
    }
}
