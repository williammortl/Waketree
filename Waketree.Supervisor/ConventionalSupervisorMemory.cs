using Waketree.Common;
using Waketree.Common.Interfaces;
using Waketree.Common.Models;

namespace Waketree.Supervisor
{
    public class ConventionalSupervisorMemory : ISupervisorMemory
    {
        private TwoDimensionalConcurrentDictionary<string, string, WaketreeMemory> memoryRegistry;

        public ConventionalSupervisorMemory()
        {
            this.memoryRegistry = new TwoDimensionalConcurrentDictionary<string, string, WaketreeMemory>();
        }

        public IEnumerable<WaketreeMemory> Memories
        {
            get
            {
                return this.memoryRegistry.AllValues;
            }
        }

        public IEnumerable<Tuple<string, string>> MemoryInfo
        {
            get
            {
                return this.memoryRegistry.AllKeyCombos;
            }
        }

        public WaketreeMemory this[Tuple<string, string> index]
        {
            get
            {
                return this[index.Item1, index.Item2];
            }
            set
            {
                this[index.Item1, index.Item2] = value;
            }
        }

        public WaketreeMemory this[string processID, string memoryName]
        {
            get
            {
                var mem = this.GetMemory(processID, memoryName);
                if (mem == null)
                {
                    throw new KeyNotFoundException(
                        string.Format("Could not find {0}, (1)",
                            processID,
                            memoryName));
                }
                return mem;
            }
            set
            {
                if (value == null)
                {
                    this.DeregisterMemory(processID, memoryName);
                    return;
                }
                this.RegisterMemory(processID, memoryName, value);
            }
        }

        public IEnumerable<string> this[string processID]
        {
            get
            {
                var memoryNames = this.memoryRegistry.Get(processID);
                return (memoryNames != null) ?
                    memoryNames.Keys : 
                    Enumerable.Empty<string>();
            }
        }

        public void RegisterMemory(WaketreeMemory mem)
        {
            this.RegisterMemory(mem.ProcessID, mem.MemoryName, mem);
        }

        public void RegisterMemory(string processID, string memoryName, WaketreeMemory mem)
        {
            this.memoryRegistry.AddOrUpdate(processID, memoryName, mem);
        }

        public void RegisterMemory(Tuple<string, string> memory, WaketreeMemory mem)
        {
            this.RegisterMemory(memory.Item1, memory.Item2, mem);
        }

        public WaketreeMemory? GetMemory(string processID, string memoryName)
        {
            return this.memoryRegistry.Get(processID, memoryName);
        }

        public WaketreeMemory? GetMemory(Tuple<string, string> memory)
        {
            return this.GetMemory(memory.Item1, memory.Item2);
        }

        public void DeregisterMemory(Tuple<string, string> memory)
        {
            this.DeregisterMemory(memory.Item1, memory.Item2);
        }

        public void DeregisterMemory(string processID, string memoryName)
        {
            this.memoryRegistry.Delete(processID, memoryName);
        }

        public void DeregisterMemory(WaketreeMemory mem)
        {
            this.DeregisterMemory(mem.ProcessID, mem.MemoryName);
        }

        public void DeregisterMemory(string processID)
        {
            this.memoryRegistry.Delete(processID);
        }
    }
}
