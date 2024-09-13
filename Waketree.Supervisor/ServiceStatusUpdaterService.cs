using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor
{
    sealed class ServiceStatusUpdaterService : IHostedService
    {
        private State state;
        private ILogger<ServiceStatusUpdaterService> logger;
        private Timer? timer = null;

        public ServiceStatusUpdaterService(ILogger<ServiceStatusUpdaterService> logger)
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
            this.logger.LogTrace("Starting the service status updater service");
            this.timer = new Timer(this.UpdateServiceStats, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.timer == null)
            {
                return Task.CompletedTask;
            }
            this.logger.LogTrace("Stopping the service status updater service");
            this.timer?.Dispose();
            this.timer = null;
            return Task.CompletedTask;
        }

        public void UpdateServiceStats(object? state)
        {
            var removeServiceKeys = new List<string>();
            Parallel.ForEach<string>(this.state.StatsServices.Keys, (key) =>
            {
                this.logger.LogTrace(
                    string.Format("Fetching stats from {0}", key));
                var stats = RESTCalls.GetServiceStats(key);
                if (stats != null)
                {
                    this.state.StatsServices[key] = stats;
                }
                else
                {
                    this.logger.LogError(
                        string.Format("Supervisor stats REST call failed when connecting to: {0}",
                        key));
                }
            });
        }
    }
}
