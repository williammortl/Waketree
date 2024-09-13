using Waketree.Service.Models;

namespace Waketree.Common.Interfaces
{
    public interface IServiceMemory
    {
        IEnumerable<string> this[string processID] { get; }

        IEnumerable<Tuple<string, string>> LocalMemoryInfo { get; }

        IEnumerable<string> LocalMemoriesByProcess(string processID);

        bool ContainsMemory(string processID);

        bool ContainsMemory(string processID, string memoryName);

        void AddOrUpdateMemory(Tuple<string, string> memory, object? mem);

        void AddOrUpdateMemory(string processID, string memoryName, object? mem);

        bool TestAndSet(Tuple<string, string> memory, object? newVal, object? expectedVal);

        bool TestAndSet(string processID, string memoryName, object? newVal, object? expectedVal);

        object? GetMemory(Tuple<string, string> memory);

        object? GetMemory(string processID, string memoryName);

        void DeleteMemory(Tuple<string, string> memory);

        void DeleteMemory(string processID, string memoryName);

        void DeleteLocalMemoryByProcessID(string processID);
    }
}
