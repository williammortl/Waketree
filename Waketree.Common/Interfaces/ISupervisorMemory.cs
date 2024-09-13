using Waketree.Common.Models;

namespace Waketree.Common.Interfaces
{
    public interface ISupervisorMemory
    {
        IEnumerable<WaketreeMemory> Memories { get; }

        IEnumerable<Tuple<string, string>> MemoryInfo { get; }

        WaketreeMemory this[Tuple<string, string> index] { get; set; }

        WaketreeMemory this[string processID, string memoryName] { get; set; }

        IEnumerable<string> this[string processID] { get; }

        void RegisterMemory(WaketreeMemory mem);

        void RegisterMemory(Tuple<string, string> memory, WaketreeMemory mem);

        void RegisterMemory(string processID, string memoryName, WaketreeMemory mem);

        WaketreeMemory? GetMemory(Tuple<string, string> memory);

        WaketreeMemory? GetMemory(string processID, string memoryName);

        void DeregisterMemory(WaketreeMemory mem);

        void DeregisterMemory(string processID);

        void DeregisterMemory(Tuple<string, string> memory);

        void DeregisterMemory(string processID, string memoryName);
    }
}
