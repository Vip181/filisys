using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filesys
{
    static class MemoryManager
    {
        static uint usedMemory = 0;

        public static void Allocate(uint bytes)
        {
            usedMemory += bytes;
        }

        public static void Free(uint bytes)
        {
            if (usedMemory >= bytes)
                usedMemory -= bytes;
        }

        public static uint UsedMemory()
        {
            return usedMemory;
        }
    }
}
