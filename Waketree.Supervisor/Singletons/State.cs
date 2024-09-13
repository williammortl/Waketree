using System.Collections.Concurrent;
using Waketree.Common;
using Waketree.Common.Interfaces;
using Waketree.Common.Models;
using Waketree.Supervisor.Models;

namespace Waketree.Supervisor.Singletons
{
    sealed class State
    {
        private static State? instance;

        private ConcurrentDictionary<string, WaketreeProcess> processByID;
        private TwoDimensionalConcurrentDictionary<string, string, WaketreeProcess> processesByIPAddress;
        private TwoDimensionalConcurrentDictionary<string, string, byte> ipAddressesByProcessID;
        private ConcurrentDictionary<string, WaketreeThread> threadByID;
        private TwoDimensionalConcurrentDictionary<string, string, WaketreeThread> threadsByIPAddress;
        private TwoDimensionalConcurrentDictionary<string, string, WaketreeThread> threadsByProcessID;

        public IHost App { get; set; }
        public ConcurrentQueue<UDPMessageWrapper> UDPMessageQueue { get; private set; }
        public ConcurrentQueue<SupervisorOperation> OperationQueue { get; private set; }
        public ConcurrentDictionary<string, StatsResponse> StatsServices { get; private set; }
        public ISupervisorMemory Memory { get; private set; }

        private State()
        {
            this.processByID = new ConcurrentDictionary<string, WaketreeProcess>();
            this.processesByIPAddress = new TwoDimensionalConcurrentDictionary<string, string, WaketreeProcess>();
            this.ipAddressesByProcessID = new TwoDimensionalConcurrentDictionary<string, string, byte>();
            this.threadByID = new ConcurrentDictionary<string, WaketreeThread>();
            this.threadsByIPAddress = new TwoDimensionalConcurrentDictionary<string, string, WaketreeThread>();
            this.threadsByProcessID = new TwoDimensionalConcurrentDictionary<string, string, WaketreeThread>();

            this.UDPMessageQueue = new ConcurrentQueue<UDPMessageWrapper>();
            this.OperationQueue = new ConcurrentQueue<SupervisorOperation>();
            this.StatsServices = new ConcurrentDictionary<string, StatsResponse>();
            var supervisorMemoryClass = Config.GetConfig().SupervisorMemory;
            this.Memory = Utils.LoadClass<ISupervisorMemory>(
                supervisorMemoryClass,
                new NullReferenceException(
                    string.Format("Could not find and load supervisor memory: {0}", supervisorMemoryClass[1])));
        }

        public static State GetState()
        {
            if (State.instance == null)
            {
                State.instance = new State();
            }
            return State.instance;
        }

        public IEnumerable<WaketreeProcess> ProcessesList
        {
            get
            {
                return this.processByID.Values;
            }
        }

        public IEnumerable<WaketreeThread> ThreadsList
        {
            get
            {
                return this.threadByID.Values;
            }
        }


        public WaketreeProcess? GetProcessByID(string processID)
        {
            this.processByID.TryGetValue(processID, out var retVal);
            return retVal;
        }

        public IEnumerable<WaketreeProcess>? GetProcessesByIPAddress(string ipAddress)
        {
            var retVal = this.processesByIPAddress.Get(ipAddress);
            return retVal?.Values.AsEnumerable<WaketreeProcess>();
        }

        public IEnumerable<string>? GetIPAddressesByProcessID(string processID)
        {
            var retVal = this.ipAddressesByProcessID.Get(processID);
            return retVal?.Keys.AsEnumerable<string>();
        }

        public void AddProcess(string ipAddress, WaketreeProcess process)
        {
            this.processByID.TryAdd(process.ProcessID, process);
            this.processesByIPAddress.AddOrUpdate(ipAddress, process.ProcessID, process);
            this.ipAddressesByProcessID.AddOrUpdate(process.ProcessID, ipAddress, 1);
        }

        public void DeleteProcess(string processID)
        {
            
            // delete process
            this.processByID.TryRemove(processID, out _);
            var ipAddresses = this.ipAddressesByProcessID.Get(processID);
            if (ipAddresses != null)
            {
                foreach (var ip in ipAddresses.Keys)
                {
                    this.processesByIPAddress.Delete(ip, processID);
                }
            }
            this.ipAddressesByProcessID.Delete(processID);

            // delete all thread records for this process
            var threadsForProcess = this.threadsByProcessID.Get(processID);
            if (threadsForProcess != null)
            {
                var threadIDs = threadsForProcess.Keys.ToArray();
                foreach (var threadID in threadIDs)
                {
                    this.DeleteThread(threadID);
                }
            }
            this.threadsByProcessID.Delete(processID);
        }

        public WaketreeThread? GetThreadByID(string threadID)
        {
            this.threadByID.TryGetValue(threadID, out var retVal);
            return retVal;
        }

        public IEnumerable<WaketreeThread>? GetThreadsByIPAddress(string ipAddress)
        {
            var retVal = this.threadsByIPAddress.Get(ipAddress);
            return retVal?.Values.AsEnumerable<WaketreeThread>();
        }

        public IEnumerable<WaketreeThread>? GetThreadsByProcessID(string processID)
        {
            var retVal = this.threadsByProcessID.Get(processID);
            return retVal?.Values.AsEnumerable<WaketreeThread>();
        }

        public void AddThread(WaketreeThread thread)
        {
            this.threadByID.TryAdd(thread.ThreadID, thread);
            this.threadsByIPAddress.AddOrUpdate(thread.IPAddress, thread.ThreadID, thread);
            this.threadsByProcessID.AddOrUpdate(thread.ProcessID, thread.ThreadID, thread);
        }

        /// <summary>
        /// Deletes a thread record
        /// </summary>
        /// <param name="threadID">thread to delete</param>
        /// <returns>a tuple of whether the process record needs to be deleted, and the process id for the deleted thread</returns>
        public Tuple<bool, string> DeleteThread(string threadID)
        {
            if (this.threadByID.TryRemove(threadID, out var threadRec))
            {
                var processID = threadRec.ProcessID;
                this.threadsByIPAddress.Delete(threadRec.IPAddress, threadRec.ThreadID);
                this.threadsByProcessID.Delete(processID, threadRec.ThreadID);

                return (this.threadsByProcessID.ContainsKey(processID)) ? 
                    new Tuple<bool, string>(false, processID) :
                    new Tuple<bool, string>(true, processID);
            }

            return new Tuple<bool, string>(false, string.Empty);
        }
    }
}