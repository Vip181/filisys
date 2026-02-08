using Cosmos.System.Graphics;
using filesys.GUI;
using System;
using System.Collections.Generic;
using sys = Cosmos.System;

namespace filesys
{
    internal class windows_option : Window
    {
        public List<Button> Buttons = new List<Button>();
        public windows_option()
          : base("exit", 200, 200, 400, 300)
        {
        }

        public override void Draw(Canvas canvas)
        {
            // Déclaration du bouton avant utilisation
            Button shutdownButton = new Button(
                X + 120,
                Y + 180,
                160,
                35,
                "Shutdown",
                () =>
                {
                    Cosmos.HAL.Power.ACPIShutdown();
                }
            );

            // Ajout du bouton à la liste des boutons
            Buttons.Add(shutdownButton);

            // Dessiner le bouton
            shutdownButton.Draw(canvas);
        }
    }
}
