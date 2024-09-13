using Waketree.Common;
using Waketree.Common.Bases;
using Waketree.Common.Interfaces;
using Waketree.Common.Models;
using Waketree.Supervisor.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor
{
    sealed class SupervisorOperationProcessorService : BaseThreadedService
    {
        private ILogger<SupervisorOperationProcessorService> logger;
        private ILoadBalancer loadBalancer;

        public SupervisorOperationProcessorService(ILogger<SupervisorOperationProcessorService> logger)
        {
            this.logger = logger;
            var loadBalancerClass = Config.GetConfig().LoadBalancer;
            this.loadBalancer = Utils.LoadClass<ILoadBalancer>(
                loadBalancerClass,
                new NullReferenceException(
                    string.Format("Could not find load balancer: {0}", loadBalancerClass[1])));
        }

        protected override void ThreadProc(CancellationToken cancellationToken)
        {
            var state = State.GetState();
            while (!cancellationToken.IsCancellationRequested)
            {
                if (state.OperationQueue.TryDequeue(out var operation))
                {
                    this.logger.LogTrace(
                        string.Format("Received {0} supervisor operation from {1}",
                            Enum.GetName(operation.Operation),
                            operation.SenderIPAddress));
                    switch (operation.Operation)
                    {
                        case SupervisorOperations.CreateProcess:
                            {
                                if (operation.Data != null)
                                {
                                    var newProcess = (WaketreeProcess)operation.Data;
                                    state.AddProcess(WaketreeConstants.SupervisorIPLookup, newProcess);

                                    // queue the first thread creation
                                    var threadCreateOperation = new SupervisorOperation()
                                    {
                                        Operation = SupervisorOperations.CreateThread,
                                        SenderIPAddress = operation.SenderIPAddress,
                                        Data = new WaketreeThread()
                                        {
                                            ProcessID = newProcess.ProcessID,
                                            ThreadID = Guid.NewGuid().ToString(),
                                            Runtime = newProcess.Runtime,
                                            ThreadStartLocation = null,
                                            ThreadFrame = null
                                        }
                                    };
                                    state.OperationQueue.Enqueue(threadCreateOperation);
                                }
                                else
                                {
                                    this.logger.LogWarning("Didn't receive a new process to create");
                                }

                                break;
                            }
                        case SupervisorOperations.KillProcess:
                            {
                                if (operation.Data != null)
                                {
                                    var processIDToKill = (string)operation.Data;
                                    UDPMessages.BroadcastKillProcess(processIDToKill);
                                    Thread.Sleep(0);
                                    state.DeleteProcess(processIDToKill);
                                    State.GetState().Memory.DeregisterMemory(processIDToKill);
                                }
                                else
                                {
                                    this.logger.LogWarning("Didn't receive a process id to kill");
                                }

                                break;
                            }
                        case SupervisorOperations.CreateThread:
                            {
                                try
                                {
                                    var newThread = (WaketreeThread)operation.Data;
                                    this.loadBalancer.WhichService(
                                        state.StatsServices,
                                        newThread);

                                    if (RESTCalls.RunThread(newThread).Success)
                                    {
                                        // TODO: make a thread info object that doesnt contain the frame
                                        state.AddThread(newThread);
                                    }
                                    else
                                    {
                                        this.logger.LogError("Could not create thread due to failed REST call");
                                    }
                                }
                                catch (NotSupportedException e)
                                {
                                    this.logger.LogError(e, "Could not create thread");
                                }

                                break;
                            }
                        case SupervisorOperations.KillThread:
                            {
                                var threadIDToKill = (string)operation.Data;
                                var threadToKill = state.GetThreadByID(threadIDToKill);
                                if (threadToKill != null)
                                {
                                    RESTCalls.KillThread(threadToKill);
                                }
                                else
                                {
                                    this.logger.LogWarning(
                                        string.Format("Couldn't find thread {0} to kill",
                                            threadIDToKill));
                                }
                                var deletionData = state.DeleteThread(threadIDToKill);
                                if (deletionData.Item1)
                                {
                                    State.GetState().OperationQueue.Enqueue(new Models.SupervisorOperation()
                                    {
                                        Operation = SupervisorOperations.KillProcess,
                                        SenderIPAddress = WaketreeConstants.SupervisorIPLookup,
                                        Data = deletionData.Item2
                                    });
                                }

                                break;
                            }
                        case SupervisorOperations.ThreadEnded:
                            {
                                if (operation.Data != null)
                                {
                                    var threadEnded = (string)operation.Data;
                                    state.DeleteThread(threadEnded);
                                }
                                else
                                {
                                    this.logger.LogWarning("Didn't receive a thread id to record as ended");
                                }

                                break;
                            }
                    }
                }
                Thread.Sleep(0);
            }
        }
    }
}
