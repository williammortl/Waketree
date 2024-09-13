namespace Waketree.Common.Models
{
    public sealed class StatsResponse
    {
        public ServiceStates State { get; set; }
        public string ComputerName { get; set; }
        public string OSPlatform { get; set; } = string.Empty;
        public string OSVersion { get; set; } = string.Empty;
        public int CpuCores { get; set; }
        public float CpuFreePercent { get; set; }
        public long TotalMemoryMB { get; set; }
        public long MemoryFreeMB { get; set; }
        public float MemoryFreePercent { get; set; }
        public float AvailabilityScore { get; set; }
        public string SupervisorIP { get; set; } = string.Empty;

        public string Memory {  get; set; }

        public string[] Runtimes { get; set; }

        public int ProcessCount { get; set; }

        public int ThreadCount { get; set; }

        public int VariableCount { get; set; }

    }
}
