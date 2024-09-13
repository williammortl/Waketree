using Waketree.Neptune.Common.Bases;

namespace Waketree.Neptune.Common.Models
{
    public class Operation
    {
        public OpCode OpCode { get; set; }
        public List<NeptuneValueBase> Parameters { get; private set; }

        public Operation()
        {
            this.Parameters = new List<NeptuneValueBase>();  
        }
    }
}
