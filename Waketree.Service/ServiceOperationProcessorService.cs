using Waketree.Common.Bases;
using Waketree.Common.Interfaces;
using Waketree.Common.Models;
using Waketree.Service.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service
{
    public class ServiceOperationProcessorService : BaseThreadedService
    {
        private ILogger<ServiceOperationProcessorService> logger;

        public ServiceOperationProcessorService(ILogger<ServiceOperationProcessorService> logger)
        {
            this.logger = logger;
        }

        protected override void ThreadProc(CancellationToken cancellationToken)
        {
            var state = State.GetState();
            while (!cancellationToken.IsCancellationRequested)
            {
                if (state.OperationQueue.TryDequeue(out var operation))
                {
                    this.logger.LogTrace(
                        string.Format("Received {0} service operation from {1}",
                            Enum.GetName(operation.Operation),
                            operation.SenderIPAddress));
                    switch (operation.Operation)
                    {
                        case ServiceOperations.KillProcess:
                            {
                                if (operation.Data != null)
                                {
                                    var processIDToKill = (string)operation.Data;
                                    state.DeleteProcess(processIDToKill);
                                    State.GetState().Memory.DeleteLocalMemoryByProcessID(processIDToKill);
                                }
                                else
                                {
                                    this.logger.LogWarning("Didn't receive a process id to kill");
                                }

                                break;
                            }
                        case ServiceOperations.RunThread:
                            {
                                if (operation.Data != null)
                                {
                                    var newThread = (ThreadRunArgument)operation.Data;
                                    var processForThread = state.GetProcessByID(newThread.ProcessID);
                                    if (processForThread == null)
                                    {
                                        processForThread = RESTCalls.GetProcessFromSupervisor(newThread.ProcessID);
                                        if (processForThread == null)
                                        {
                                            this.logger.LogWarning(
                                                string.Format("Couldn't start thread {0} because could not access process data for process {1}",
                                                    newThread.ThreadID,
                                                    newThread.ProcessID));
                                            break;
                                        }
                                    }

                                    IRuntime runtime;
                                    try
                                    {
                                        runtime = Runtime.GetRuntime(newThread.Runtime);
                                    }
                                    catch (NullReferenceException e)
                                    {
                                        this.logger.LogError(e, "Could not load runtime for running a thread");
                                        break;
                                    }

                                    runtime.RunThread(processForThread, newThread);
                                    state.AddProcess(processForThread);
                                    state.AddThread(newThread);
                                }
                                else
                                {
                                    this.logger.LogWarning("Didn't receive any information for the thread to run");
                                }

                                break;
                            }
                        case ServiceOperations.KillThread:
                            {
                                if (operation.Data != null)
                                {
                                    var threadIDToKill = (string)operation.Data;
                                    var threadToKill = state.GetThreadByID(threadIDToKill);
                                    if (threadToKill == null)
                                    {
                                        this.logger.LogWarning(
                                            string.Format("Couldn't kill thread {0} because could not find thread data",
                                                threadIDToKill));
                                        break;
                                    }

                                    IRuntime runtime;
                                    try
                                    {
                                        runtime = Runtime.GetRuntime(threadToKill.Runtime);
                                    }
                                    catch (NullReferenceException e)
                                    {
                                        this.logger.LogError(e, "Could not load runtime for running a thread");
                                        break;
                                    }

                                    runtime.KillThread(threadToKill.ThreadID);
                                    var deletionData = state.DeleteThread(threadIDToKill);
                                    if (deletionData.Item1)
                                    {
                                        State.GetState().OperationQueue.Enqueue(new ServiceOperation()
                                        {
                                            Operation = ServiceOperations.KillProcess,
                                            SenderIPAddress = operation.SenderIPAddress,
                                            Data = deletionData.Item2
                                        });
                                    }
                                }
                                else
                                {
                                    this.logger.LogWarning("Didn't receive a thread id to kill");
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
