using Waketree.Service.Models;

namespace Waketree.Common.Models
{
    public class WaketreeMemory
    {
        public string ProcessID { get; set; }
        public string MemoryName { get; set; }
        public string MemoryLocation { get; set; }

        public WaketreeMemory()
        {
            this.ProcessID = string.Empty;
            this.MemoryName = string.Empty;
            this.MemoryLocation = string.Empty;
        }

        public WaketreeMemory(WaketreeMemoryValue memoryValue)
        {
            this.ProcessID = memoryValue.ProcessID;
            this.MemoryName = memoryValue.MemoryName;
            this.MemoryLocation = memoryValue.MemoryLocation;
        }
    }
}
