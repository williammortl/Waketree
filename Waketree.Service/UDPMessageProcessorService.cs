using Waketree.Common.Models;
using Waketree.Service.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service
{
    public class UDPMessageProcessorService : IHostedService
    {
        private State state;
        private ILogger<UDPMessageProcessorService> logger;
        private Timer? timer = null;

        public UDPMessageProcessorService(ILogger<UDPMessageProcessorService> logger)
        {
            this.state = State.GetState();
            this.logger = logger;  
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.timer != null)
            {
                return Task.CompletedTask;
            }
            this.logger.LogTrace("Starting the UDP message processor service");
            this.timer = new Timer(this.ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.timer == null)
            {
                return Task.CompletedTask;
            }
            this.logger.LogTrace("Stopping the UDP message processor service");
            this.timer?.Dispose();
            this.timer = null;
            return Task.CompletedTask;
        }

        private void ProcessQueue(object? state)
        {
            var messageWrapper = new UDPMessageWrapper();
            while (this.state.UDPMessageQueue.TryDequeue(out messageWrapper))
            {
                this.logger.LogTrace(
                    string.Format("Received {0} udp message from {1}",
                        Enum.GetName(messageWrapper.Message.Topic),
                        messageWrapper.SenderIPAddress));
                switch (messageWrapper?.Message?.Topic)
                {
                    case Common.UDPMessageTopic.ServiceQuery:
                        {
                            UDPMessages.BroadcastWhoIsTheSupervisor();
                            break;
                        }
                    case Common.UDPMessageTopic.Shutdown:
                        {
                            if (this.state.ServiceState == Waketree.Common.ServiceStates.Connected)
                            {
                                //UDPMessages.ServiceDisconnecting(this.state.SupervisorIP);
                            }
                            //this.state.App.StopAsync();
                            break;
                        }
                    case Common.UDPMessageTopic.KillProcess:
                        {
                            if ((messageWrapper.Message.Data != null) && 
                                (messageWrapper.Message.Data != string.Empty))
                            {
                                State.GetState().OperationQueue.Enqueue(new ServiceOperation()
                                {
                                    Operation = ServiceOperations.KillProcess,
                                    SenderIPAddress = messageWrapper.SenderIPAddress,
                                    Data = messageWrapper.Message.Data
                                });
                            }
                            else
                            {
                                this.logger.LogWarning("Didn't receive a process id to kill in the udp message");
                            }

                            break;
                        }
                }
            }
        }
    }
}
