using Cosmos.Core.Memory;
using System;
using System.Collections.Generic;

namespace filesys.System
{
    public static unsafe class OsMemoryManager
    {
        private static bool initialized;

        private static Dictionary<int, AppMemoryContext> apps =
            new Dictionary<int, AppMemoryContext>();

        private static int nextAppId = 1;
        public static int GetTotalAllocations()
        {
            int total = 0;
            foreach (var app in apps.Values)
                total += app.AllocationCount;
            return total;
        }
        public static void Init()
        {
            if (initialized) return;

            Heap.Init();
            apps.Clear();

            initialized = true;
        }

        // 🔹 Création d’un contexte mémoire pour une app / fenêtre
        public static AppMemoryContext CreateApp(string name)
        {
            int id = nextAppId++;
            var ctx = new AppMemoryContext(id, name);
            apps[id] = ctx;
            return ctx;
        }

        // 🔹 Allocation associée à une app
        public static byte* Alloc(AppMemoryContext ctx, uint size)
        {
            if (!initialized || ctx == null)
                throw new Exception("MemoryManager not initialized or context null");

            void* ptr = Heap.Alloc(size);
            if (ptr == null) return null;

            ctx.Add((IntPtr)ptr);
            return (byte*)ptr;
        }

        // 🔹 Libération explicite
        public static void Free(AppMemoryContext ctx, byte* ptr)
        {
            if (!initialized || ctx == null || ptr == null)
                return;

            IntPtr iptr = (IntPtr)ptr;
            Heap.Free(ptr);
            ctx.Remove(iptr);
        }

        // 🔹 Fermeture d’une app / fenêtre (NETTOYAGE TOTAL)
        public static void DestroyApp(AppMemoryContext ctx)
        {
            if (!initialized || ctx == null)
                return;

            foreach (var ptr in ctx.GetAll())
            {
                Heap.Free((void*)ptr);
            }

            apps.Remove(ctx.Id);
        }

        // 🔹 Nettoyage global occasionnel
        public static void Collect()
        {
            Heap.Collect();
        }

        // 🔹 Infos debug
        public static string GetStatus()
        {
            string s = "Memory contexts:\n";

            foreach (var app in apps.Values)
            {
                s += $"{app.Name} -> {app.AllocationCount} allocs\n";
            }

            return s;
        }
    }
}
 