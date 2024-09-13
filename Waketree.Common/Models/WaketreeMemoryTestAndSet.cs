using Waketree.Service.Models;

namespace Waketree.Common.Models
{
    public class WaketreeMemoryTestAndSet : WaketreeMemoryValue
    {
        public ComplexSerializableObject? MemoryExpectedValue { get; set; }

        public WaketreeMemoryTestAndSet() :
            base()
        {
            this.MemoryExpectedValue = null;
        }
    }
}
