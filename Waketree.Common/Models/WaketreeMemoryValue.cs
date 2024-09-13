using Waketree.Common.Models;

namespace Waketree.Service.Models
{
    public class WaketreeMemoryValue : WaketreeMemory
    {
        public ComplexSerializableObject? MemoryValue { get; set; }

        public WaketreeMemoryValue()
            : base()
        {
            this.MemoryValue = null;
        }

        public WaketreeMemoryValue(WaketreeMemory memory)
        {
            this.ProcessID = memory.ProcessID;
            this.MemoryName = memory.MemoryName;
            this.MemoryLocation = memory.MemoryLocation;
            this.MemoryValue = null;
        }
    }
}
