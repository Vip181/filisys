using System;
using System.Collections.Generic;
using Cosmos.System;
using Cosmos.Core.Memory;
namespace filesys.System
{
    public class ProcessMemoryManager
    {
        // Liste des adresses mémoires allouées pour ce processus
        private List<IntPtr> allocations;
        public string ProcessName { get; private set; }

        public ProcessMemoryManager(string name)
        {
            this.ProcessName = name;
            this.allocations = new List<IntPtr>();
        }

        /// <summary>
        /// Alloue un bloc de mémoire et le suit pour ce programme
        /// </summary>
        /// <param name="size">Taille en bytes</param>
        public IntPtr Allocate(uint size)
        {
            // On utilise l'allocateur natif de Cosmos
            IntPtr ptr = (IntPtr)Cosmos.Core.GCImplementation.AllocNewObject(size);

            if (ptr != IntPtr.Zero)
            {
                allocations.Add(ptr);
            }

            return ptr;
        }

        /// <summary>
        /// Libère toute la mémoire associée à ce programme (le "Reset")
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var ptr in allocations)
            {
                // Note : Free est disponible dans ta version (image_a772ba.png)
                Cosmos.Core.GCImplementation.Free(ptr);
            }
            allocations.Clear();
        }

        public int GetAllocationCount() => allocations.Count;
    }
}