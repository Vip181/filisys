using System;
using System.Collections.Generic;

namespace filesys.System
{
    public class AppMemoryContext
    {
        public int Id { get; }
        public string Name { get; }

        private List<IntPtr> allocations = new List<IntPtr>();

        public AppMemoryContext(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void Add(IntPtr ptr)
        {
            allocations.Add(ptr);
        }

        public void Remove(IntPtr ptr)
        {
            allocations.Remove(ptr);
        }

        public IEnumerable<IntPtr> GetAll()
        {
            return allocations;
        }

        public int AllocationCount => allocations.Count;
    }
}
