using System.Collections.Concurrent;
using Waketree.Common;
using Waketree.Common.Interfaces;
using Waketree.Common.Models;
using Waketree.Service.Models;

namespace Waketree.Service.Singletons
{
    sealed class State
    {
        private static State? instance;

        private DateTime lastSupervisorContact;
        private ServiceStates serviceState;
        private ConcurrentDictionary<string, WaketreeThread> threadByID;
        private ConcurrentDictionary<string, ConcurrentDictionary<string, WaketreeThread>> threadsByProcessID;
        private ConcurrentDictionary<string, WaketreeProcess> processByID;

        public IHost App { get; set; }
        public Config Config { get; private set; } = Config.GetConfig();
        public string SupervisorIP { get; set; } = string.Empty;
        public ConcurrentQueue<UDPMessageWrapper> UDPMessageQueue { get; private set; }
        public ConcurrentQueue<ServiceOperation> OperationQueue { get; private set; }
        public IServiceMemory Memory { get; private set; }

        private State()
        {
            this.ServiceState = ServiceStates.Down;
            this.threadByID = new ConcurrentDictionary<string, WaketreeThread>();
            this.threadsByProcessID = new ConcurrentDictionary<string, 
                ConcurrentDictionary<string, WaketreeThread>>();
            this.processByID = new ConcurrentDictionary<string, WaketreeProcess>();

            this.UDPMessageQueue = new ConcurrentQueue<UDPMessageWrapper>();
            this.OperationQueue = new ConcurrentQueue<ServiceOperation>();
            var serviceMemoryClass = Config.GetConfig().ServiceMemory;
            this.Memory = Utils.LoadClass<IServiceMemory>(
                serviceMemoryClass,
                new NullReferenceException(
                    string.Format("Could not find and load service memory: {0}", serviceMemoryClass[1])));
        }

        public static State GetState()
        {
            if (State.instance == null)
            {
                State.instance = new State();
            }
            return State.instance;
        }

        public ServiceStates ServiceState
        {
            get
            {
                return this.serviceState;
            }
            set
            {
                if (value != ServiceStates.Connected)
                {
                    this.lastSupervisorContact = DateTime.Now.Subtract(new TimeSpan(0,
                        Config.GetConfig().MaxSupervisorContactMinutes,
                        0));
                    this.SupervisorIP = string.Empty;
                }
                this.serviceState = value;
            }
        }

        public bool SupervisorContactExpired
        {
            get
            {
                return (DateTime.Now - lastSupervisorContact).TotalMinutes > Config.GetConfig().MaxSupervisorContactMinutes;
            }
        }

        public IEnumerable<string> ProcessesList
        {
            get
            {
                return this.threadsByProcessID.Keys.AsEnumerable<string>();
            }
        }

        public IEnumerable<WaketreeThread> ThreadsList
        {
            get
            {
                return this.threadByID.Values;
            }
        }

        public void TalkedToSupervisor()
        {
            lastSupervisorContact = DateTime.Now;
        }

        public IEnumerable<WaketreeThread>? GetThreadsByProcessID(string processID)
        {
            this.threadsByProcessID.TryGetValue(processID, out var retVal);
            return retVal?.Values.ToList<WaketreeThread>();
        }

        public WaketreeThread? GetThreadByID(string threadID)
        {
            this.threadByID.TryGetValue(threadID, out var retVal);
            return retVal;
        }

        public void AddThread(WaketreeThread thread)
        {
            // add thread
            this.threadByID.TryAdd(thread.ThreadID, thread);
            if (this.threadsByProcessID.TryGetValue(thread.ProcessID, out var processIDThreadDictionary))
            {
                processIDThreadDictionary.TryAdd(thread.ThreadID, thread);
            }
            else
            {
                this.threadsByProcessID.TryAdd(thread.ProcessID, new ConcurrentDictionary<string, WaketreeThread>(
                    new List<KeyValuePair<string, WaketreeThread>>() {
                        new KeyValuePair<string, WaketreeThread>(
                            thread.ThreadID,
                            thread) }));
            }
        }

        /// <summary>
        /// Deletes a thread record
        /// </summary>
        /// <param name="threadID">thread to delete</param>
        /// <returns>a tuple of whether the process record needs to be deleted, and the process id for the deleted thread</returns>
        public Tuple<bool, string> DeleteThread(string threadID)
        {
            bool deleteProcess = false;
            var processID = string.Empty;

            if (this.threadByID.TryRemove(threadID, out var threadRec))
            {
                processID = threadRec.ProcessID;
                if (this.threadsByProcessID.TryGetValue(processID, out var threadDic))
                {
                    threadDic.TryRemove(threadID, out _);
                    deleteProcess = (threadDic.Keys.Count == 0);
                }
            }

            return new Tuple<bool, string>(deleteProcess, processID);
        }

        public WaketreeProcess? GetProcessByID(string processID)
        {
            this.processByID.TryGetValue(processID, out var process);
            return process;
        }

        public void AddProcess(WaketreeProcess process)
        {
            this.processByID.TryAdd(process.ProcessID, process);
        }

        public void DeleteProcess(string processID)
        {
            this.processByID.TryRemove(processID, out _);
            this.threadsByProcessID.TryRemove(processID, out var threadDict);

            // delete all thread records for this process
            if (threadDict != null)
            {
                var threadIDs = threadDict.Keys.ToArray();
                foreach (var threadID in threadIDs)
                {
                    this.DeleteThread(threadID);
                }
            }
        }
    }
}
