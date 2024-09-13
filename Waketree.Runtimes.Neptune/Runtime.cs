using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Waketree.Common.Interfaces;
using Waketree.Common.Models;
using Waketree.Neptune.Common.Interfaces;
using Waketree.Neptune.Common.Models;
using Waketree.Neptune.Interpreter;

namespace Waketree.Runtimes.Neptune
{
    public class Runtime : IRuntime 
    {
        // TODO: implement static bytecode cache? 

        private string applicationRootDirectory;
        private ILogger<IRuntime> logger;
        private IServiceInterop serviceInterop;
        private ConcurrentDictionary<string, Tuple<WaketreeProcess, WaketreeThread, int, CancellationTokenSource>> threadLookup;

        public string RuntimeString {get; private set;}

        public Runtime(string runtimeString,
            string applicationRootDirectory,
            IServiceInterop interop,
            ILogger<IRuntime> logger)
        {
            this.RuntimeString = runtimeString;
            this.applicationRootDirectory = applicationRootDirectory;
            this.serviceInterop = interop;
            this.logger = logger;
            this.threadLookup = new ConcurrentDictionary<string, Tuple<WaketreeProcess, WaketreeThread, int, CancellationTokenSource>>();
        }

        public void KillThread(string threadID)
        {
            if (this.threadLookup.TryGetValue(threadID, out var threadRec))
            {
                threadRec.Item4.Cancel();
                Thread.Sleep(0);

                this.threadLookup.TryRemove(threadID, out _);
            }
            else
            {
                this.logger.LogWarning(string.Format("{0} could not kill thread {1}",
                    "Neptune runtime",
                    threadID));
            }
        }

        // TODO: should I put the file in the thread object for applications that contain multiple bytecode files?
        public bool RunThread(WaketreeProcess process, WaketreeThread thread)
        {
            var fullFilename = Path.Combine(this.applicationRootDirectory, process.Filename);
            this.logger.LogWarning(
                string.Format("{0}|{1}|{2}",
                    this.applicationRootDirectory,
                    process.Filename,
                    fullFilename));
            var bytecode = Parser.ParseBytecode(fullFilename);
            var console = new LoggerConsole(this.logger, thread);
            var cancelSource = new CancellationTokenSource();

            // distributed memory frame
            WaketreeNeptuneFrame frame;
            if (thread.ThreadFrame == null)
            {
                frame = new WaketreeNeptuneFrame(
                    this.serviceInterop.Memory,
                    process.ProcessID,
                    bytecode);

                // add in command line arguments
                frame.StoreGlobal(-1, new NeptuneValueString()
                {
                    Value = process.Filename
                });
                var counter = -2;
                foreach (var commandArg in process.CommandArgs)
                {
                    frame.StoreGlobal(counter, new NeptuneValueString()
                    {
                        Value = commandArg
                    });
                    counter--;
                }
            }
            else
            {
                var frameData = thread.ThreadFrame.DeserializeObject();
                if (frameData != null)
                {
                    frame = new WaketreeNeptuneFrame((NeptuneFrameData)frameData, this.serviceInterop.Memory);
                }
                else
                {
                    throw new ArgumentNullException(
                        string.Format("Frame data was passed for process: {0}, thread: {1}, runtime: {2}, but was deserialized to null",
                            thread.ProcessID,
                            thread.ThreadID,
                            this.RuntimeString));
                }
            }

            var interpreterThread = new Thread(() =>
            {
                (new Interpreter(
                    thread.ProcessID,
                    console,
                    bytecode,
                    this.NewInterpreterThreadCallback)).Interpret(
                        frame,
                        (thread.ThreadStartLocation == null) ? 0 : Convert.ToInt32(thread.ThreadStartLocation),
                        cancelSource.Token);
                this.serviceInterop.WaketreeThreadEnded(thread.ThreadID);
            });

            var newRecord = new Tuple<WaketreeProcess, WaketreeThread, int, CancellationTokenSource>(
                process,
                thread,
                interpreterThread.ManagedThreadId,
                cancelSource);

            if (!this.threadLookup.TryAdd(thread.ThreadID, newRecord))
            {
                this.logger.LogWarning(string.Format("{0} could not add record for thread {1}",
                    "Neptune runtime",
                    thread.ThreadID));
                return false;
            }

            // execute first new thread
            interpreterThread.Start();

            return true;
        }

        public void NewInterpreterThreadCallback(string processID, IConsole console, IFrame childFrame, int atAddress)
        {
            this.serviceInterop.CreateWaketreeThread(processID,
                this.RuntimeString,
                atAddress.ToString(),
                childFrame.GetFrameData());
        }
    }
}
