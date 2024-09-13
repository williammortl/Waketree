using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Interfaces;
using Waketree.Neptune.Common.Models;

namespace Waketree.Neptune.Interpreter.Bases
{
    public abstract class BaseFrame : IFrame
    {
        protected Dictionary<string, int> labelLookup;
        protected Dictionary<long, NeptuneValueBase> localVariables;
        protected Stack<NeptuneValueBase> stack;

        public BaseFrame()
        {
            this.labelLookup = new Dictionary<string, int>();
            this.localVariables = new Dictionary<long, NeptuneValueBase>();
            this.stack = new Stack<NeptuneValueBase>();
        }

        public BaseFrame(NeptuneFrameData frameData)
        {
            this.labelLookup = frameData.LabelLookup;
            this.localVariables = frameData.LocalVariables;
            this.stack = frameData.Stack;
        }

        public BaseFrame(List<Operation> bytecode) :
            this()
        {
            labelLookup = InterpreterUtils.BuildLabelLookup(bytecode);
        }

        public BaseFrame(BaseFrame parent)
        {
            labelLookup = parent.labelLookup;
            localVariables = new Dictionary<long, NeptuneValueBase>();
            stack = parent.stack.DeepCloneStack();
        }

        public NeptuneValueBase? GetLocal(long name)
        {
            if (localVariables.ContainsKey(name))
            {
                return localVariables[name];
            }
            return null;
        }

        public int LookupLabel(string label)
        {
            if (!labelLookup.ContainsKey(label))
            {
                return -1;
            }
            return labelLookup[label];
        }

        public NeptuneValueBase Pop()
        {
            return stack.Pop();
        }

        public void Push(NeptuneValueBase data)
        {
            stack.Push(data);
        }

        public void StoreLocal(long name, NeptuneValueBase? data)
        {
            if (data == null)
            {
                localVariables.Remove(name, out _);
            }
            else
            {
                localVariables[name] = data;
            }
        }

        public abstract bool EnterCriticalSection(long name);

        public abstract void ExitCriticalSection(long name);

        public abstract IFrame GetChildFrame();

        public abstract NeptuneValueBase? GetGlobal(long name);

        public abstract void StoreGlobal(long name, NeptuneValueBase? data);

        public virtual NeptuneFrameData GetFrameData()
        {
            return new NeptuneFrameData()
            {
                ProcessID = string.Empty,
                LabelLookup = this.labelLookup,
                LocalVariables = this.localVariables,
                Stack = this.stack
            };
        }
    }
}
