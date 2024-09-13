namespace Waketree.Supervisor
{
    sealed class WebService : IHostedService
    {
        private readonly ILogger<WebService> logger;
        private WebApplication? webApp;

        public WebService(ILogger<WebService> logger)
        {
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogTrace("Starting the web application");
            var builderWebApp = WebApplication.CreateBuilder();
            builderWebApp.Services.AddControllers();
            builderWebApp.Services.AddEndpointsApiExplorer();
            builderWebApp.Services.AddSwaggerGen();
            this.webApp = builderWebApp.Build();
            webApp.UseSwagger();
            webApp.UseSwaggerUI();
            webApp.MapControllers();
            webApp.RunAsync();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogTrace("Stopping the web application");
            this.webApp?.StopAsync();
            this.webApp?.DisposeAsync();
            this.webApp = null;
            return Task.CompletedTask;
        }
    }
}
