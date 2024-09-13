using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor
{
    sealed class UDPMessageProcessorService : IHostedService
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
                switch (messageWrapper.Message?.Topic)
                {
                    case Common.UDPMessageTopic.Shutdown:
                        {
                            UDPMessages.BroadcastShutdownToServices();
                            this.state.App.StopAsync();
                            break;
                        }
                    case Common.UDPMessageTopic.SupervisorQuery:
                        {
                            if (messageWrapper != null)
                            {
                                // connect to client
                                var clonedMessageWrapper = (UDPMessageWrapper)messageWrapper.Clone();
                                new Thread(() =>
                                {
                                    if (RESTCalls.SupervisorConnect(clonedMessageWrapper.SenderIPAddress))
                                    {
                                        var stats = RESTCalls.GetServiceStats(clonedMessageWrapper.SenderIPAddress);
                                        if (stats != null)
                                        {
                                            this.state.StatsServices[clonedMessageWrapper.SenderIPAddress] = stats;
                                        }
                                        else
                                        {
                                            this.logger.LogError(
                                                string.Format("Supervisor stats REST call failed when connecting to: {0}",
                                                clonedMessageWrapper.SenderIPAddress));
                                        }
                                    }
                                    else
                                    {
                                        this.logger.LogError(
                                            string.Format("Supervisor query couldn't connect to: {0}",
                                            clonedMessageWrapper.SenderIPAddress));
                                    }
                                }).Start();
                            }
                            else
                            {
                                this.logger.LogWarning("Received empty sender address for supervisor query");
                            }
                            break;
                        }
                    case Common.UDPMessageTopic.ServiceDisconnecting:
                        {
                            if (messageWrapper != null)
                            {
                                this.logger.LogInformation(
                                    string.Format("Service {0} disconnected",
                                    messageWrapper?.SenderIPAddress));
                                this.state.StatsServices.TryRemove(messageWrapper.SenderIPAddress, out var stats);
                            }
                            break;
                        }
                }
            }
        }
    }
}
