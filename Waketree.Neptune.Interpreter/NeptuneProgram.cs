using Waketree.Neptune.Common.Exceptions;
using Waketree.Neptune.Common.Interfaces;
using Waketree.Neptune.Common.Models;

namespace Waketree.Neptune.Interpreter
{
    public class NeptuneProgram
    {
        private IConsole console;
        private List<Operation> bytecode;
        private Dictionary<int, CancellationTokenSource> cancellationTokenSource;

        public NeptuneProgram(
            IConsole console,
            List<Operation> bytecode)
        {
            this.console = console;
            this.bytecode = bytecode;
            this.cancellationTokenSource = new Dictionary<int, CancellationTokenSource>();
        }

        public List<int> RunningThreads
        {
            get
            {
                return this.cancellationTokenSource.Keys.ToList<int>();
            }
        }

        /// <summary>
        /// Creates a new thread for the interpreter to execute
        /// </summary>
        /// <param name="args">cammand line args</param>
        /// <param name="frame">frame</param>
        public void Execute(string[]? args, IFrame frame)
        {
            this.Execute(args, frame, 0);
        }

        /// <summary>
        /// Creates a new thread for the interpreter to execute
        /// </summary>
        /// <param name="args">cammand line args</param>
        /// <param name="frame">frame</param>
        /// <param name="label">what label to begin execution at</param>
        /// <exception cref="LabelNotFoundException"></exception>
        public void Execute(string[]? args, IFrame frame, string label)
        {
            var address = frame.LookupLabel(label);
            if (address == -1)
            {
                throw new LabelNotFoundException(
                    string.Format("Cannot execute starting at label {0}, the label was not found", label));
            }

            this.Execute(args, frame, address);
        }

        /// <summary>
        /// Creates a new thread for the interpreter to execute
        /// </summary>
        /// <param name="args">cammand line args</param>
        /// <param name="frame">frame</param>
        /// <param name="address">what address to start execution at</param>
        public void Execute(string[]? args, IFrame frame, int address)
        {
            // add args as global variables -1, ..., -n
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    frame.StoreGlobal(-1 * (1 + i),
                        new NeptuneValueString()
                        {
                            Value = args[i]
                        });
                }
            }
            
            this.NewInterpreterThread(
                Guid.NewGuid().ToString(),
                this.console, 
                frame, 
                address);
        }

        public void KillProgram()
        {
            var keysCopy = (int[])this.cancellationTokenSource.Keys.ToArray<int>().Clone();
            foreach (var key in keysCopy)
            {
                this.KillThread(key);
            }
        }

        public void KillThread(int threadID)
        {
            if (this.cancellationTokenSource.ContainsKey(threadID))
            {
                this.cancellationTokenSource[threadID].Cancel();
                Thread.Sleep(0);
                this.cancellationTokenSource[threadID].Dispose();
                this.cancellationTokenSource.Remove(threadID);
            }
        }

        private void NewInterpreterThread(string processID, IConsole console, IFrame childFrame, int atAddress)
        {
            var newCancellationTokenSource = new CancellationTokenSource();
            var newThread = (new Thread(() =>
            {
                (new Interpreter(
                    processID,
                    console,
                    this.bytecode,
                    this.NewInterpreterThread
                    )).Interpret(
                        childFrame,
                        atAddress,
                        newCancellationTokenSource.Token);
                this.ThreadCompleted(Thread.CurrentThread.ManagedThreadId);
            }));
            this.cancellationTokenSource.TryAdd(newThread.ManagedThreadId, newCancellationTokenSource);
            newThread.Start();
        }

        private void ThreadCompleted(int managedThreadID)
        {
            this.cancellationTokenSource.Remove(managedThreadID);
        }
    }
}
