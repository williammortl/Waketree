using Waketree.Service.Singletons;

namespace Waketree.Service
{
    public class SupervisorConnectionService : IHostedService
    {
        private State state;
        private ILogger<SupervisorConnectionService> logger;
        private Timer? timer = null;

        public SupervisorConnectionService(ILogger<SupervisorConnectionService> logger)
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
            this.logger.LogTrace("Starting the supervisor connection checker service");
            this.timer = new Timer(this.CheckForSupervisor, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.timer == null)
            {
                return Task.CompletedTask;
            }
            this.logger.LogTrace("Stopping the supervisor connection checker service");
            this.timer?.Dispose();
            this.timer = null;
            return Task.CompletedTask;
        }

        private void CheckForSupervisor(object? state)
        {
            if (this.state.SupervisorContactExpired)
            {
                // disconnect from supervisor
                this.state.ServiceState = Common.ServiceStates.Up;

                // message supervisor
                UDPMessages.BroadcastWhoIsTheSupervisor();
            }
        }
    }
}

