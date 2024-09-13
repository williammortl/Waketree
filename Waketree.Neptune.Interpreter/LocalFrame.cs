using System.Collections.Concurrent;
using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Interfaces;
using Waketree.Neptune.Common.Models;
using Waketree.Neptune.Interpreter.Bases;

namespace Waketree.Neptune.Interpreter
{
    public sealed class LocalFrame : BaseFrame
    {
        private ConcurrentDictionary<long, NeptuneValueBase> globalVariables;
        private ConcurrentDictionary<long, CriticalSection> criticalSections;

        public LocalFrame() : 
            base()
        {
            this.globalVariables = new ConcurrentDictionary<long, NeptuneValueBase>();
            this.criticalSections = new ConcurrentDictionary<long, CriticalSection>();
        }

        public LocalFrame(List<Operation> bytecode) :
            base(bytecode)
        {
            this.globalVariables = new ConcurrentDictionary<long, NeptuneValueBase>();
            this.criticalSections = new ConcurrentDictionary<long, CriticalSection>();
        }

        private LocalFrame(LocalFrame parent) :
            base(parent)
        {
            this.globalVariables = parent.globalVariables;
            this.criticalSections = parent.criticalSections;
        }

        public override IFrame GetChildFrame()
        {
            return new LocalFrame(this);
        }

        public override NeptuneValueBase? GetGlobal(long name)
        {
            if (this.globalVariables.ContainsKey(name))
            {
                this.globalVariables.TryGetValue(name, out var retVal);
                return retVal;
            }
            return null;
        }

        public override void StoreGlobal(long name, NeptuneValueBase? data)
        {
            if (data == null)
            {
                this.globalVariables.Remove(name, out _);
            }
            else
            {
                this.globalVariables[name] = data;
            }
        }

        public override bool EnterCriticalSection(long name)
        {
            CriticalSection critVal;
            bool addNewCritVal = false;
            if (!this.criticalSections.TryGetValue(name, out critVal))
            {
                critVal = new CriticalSection();
                addNewCritVal = true;
            }

            lock (critVal.Lock)
            {

                // new critical section
                if (addNewCritVal)
                {
                    this.criticalSections.TryAdd(name, critVal);
                    return true;
                }

                // check to make sure critical secition is available
                var retVal = critVal.Value;
                critVal.Value = false;
                return retVal;
            }
        }

        public override void ExitCriticalSection(long name)
        {
            if (this.criticalSections.ContainsKey(name))
            {
                lock (this.criticalSections[name].Lock)
                {
                    this.criticalSections[name].Value = true;
                }
            }
        }
    }
}
