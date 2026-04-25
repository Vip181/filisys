using Cosmos.System.Graphics;

namespace filesys.GUI
{
    public class WindowManager
    {
        // On n'utilise plus de liste privée ici, on utilise Kernel.Instance.windows
        public void Draw(Canvas canvas)
        {
            // On dessine chaque fenêtre de la liste globale
            foreach (var w in Kernel.Instance.windows)
            {
                // OPTIMISATION MÉMOIRE/CPU : 
                // Si réduit, on ne déclenche même pas le code de dessin
                if (w.IsMinimized || w.IsClosed) continue;

                w.Draw(canvas);
            }
        }

        // Note: La méthode Add n'est plus nécessaire ici si tu utilises Kernel.Instance.AddWindow
        public void Add(BaseWindow window) => Kernel.Instance.AddWindow(window);
    }
}