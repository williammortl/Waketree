using Newtonsoft.Json;
using Waketree.Common.Interfaces;
using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Interfaces;
using Waketree.Neptune.Common.Models;
using Waketree.Neptune.Interpreter.Bases;

namespace Waketree.Runtimes.Neptune
{
    public sealed class WaketreeNeptuneFrame : BaseFrame
    {
        private const int CriticalSectionBase = Int32.MaxValue - 100000; // allows for 100,000 critical sections in any process

        private IServiceMemory memory;
        private string processID;

        public WaketreeNeptuneFrame(IServiceMemory memory, 
            string processID, 
            List<Operation> bytecode) :
            base(bytecode)
        {
            this.memory = memory;
            this.processID = processID;
        }

        public WaketreeNeptuneFrame(NeptuneFrameData frameData, IServiceMemory memory) :
            base(frameData)
        {
            this.processID = frameData.ProcessID;
            this.memory = memory;
        }

        private WaketreeNeptuneFrame(WaketreeNeptuneFrame parent) :
            base(parent)
        {
            this.memory = parent.memory;
            this.processID = parent.processID;
        }

        public override bool EnterCriticalSection(long name)
        {
            // NOTE: stored as global variable: CriticalSectionBase + name
            var critSectionName = (WaketreeNeptuneFrame.CriticalSectionBase + name).ToString();
            var newVal = new NeptuneValueByte()
            {
                Value = 0
            };
            var expectedVal = new NeptuneValueByte()
            {
                Value = 1
            };

            return this.memory.TestAndSet(this.processID, critSectionName, newVal, expectedVal);
        }

        public override void ExitCriticalSection(long name)
        {
            // NOTE: stored as global variable: CriticalSectionBase + name
            var critSectionName = (WaketreeNeptuneFrame.CriticalSectionBase + name).ToString();
            var newVal = new NeptuneValueByte()
            {
                Value = 1
            };
            var expectedVal = new NeptuneValueByte()
            {
                Value = 0
            };

            this.memory.TestAndSet(this.processID, critSectionName, newVal, expectedVal);
        }

        public override IFrame GetChildFrame()
        {
            return new WaketreeNeptuneFrame(this);
        }

        public override NeptuneValueBase? GetGlobal(long name)
        {
            var memory = this.memory.GetMemory(this.processID, name.ToString());
            if (memory == null)
            {
                return null;
            }
            if (memory is not NeptuneValueBase)
            {
                throw new InvalidCastException("Waketree Neptune Frame requires a NeptuneValueBase memory value");
            }
            return (NeptuneValueBase)memory;
        }

        public override void StoreGlobal(long name, NeptuneValueBase? data)
        {
            if (data == null)
            {
                this.memory.DeleteMemory(this.processID, name.ToString());
            }
            else
            {
                this.memory.AddOrUpdateMemory(this.processID, name.ToString(), data);
            }
        }

        public override NeptuneFrameData GetFrameData()
        {
            var retVal = base.GetFrameData();
            retVal.ProcessID = this.processID;
            return retVal;
        }
    }
}
