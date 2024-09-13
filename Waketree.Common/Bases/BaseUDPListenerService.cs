using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Waketree.Common.Bases
{
    public abstract class BaseUDPListenerService : BaseThreadedService
    {
        private readonly ILogger<BaseUDPListenerService> logger;
        private IPEndPoint ip;
        private bool newThread;
        private UdpClient? udpClient;

        public BaseUDPListenerService(ILogger<BaseUDPListenerService> logger,
            IPEndPoint ip,
            bool newThread) : base()
        {
            this.logger = logger;
            this.ip = ip;
            this.newThread = newThread;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.udpClient != null)
            {
                return Task.CompletedTask;
            }
            this.logger.LogInformation("Starting a UDP listener");
            this.udpClient = new UdpClient(this.ip);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.udpClient == null)
            {
                return Task.CompletedTask;
            }
            this.logger.LogInformation("Stopping a UDP listener");
            this.udpClient?.Close();
            this.udpClient?.Dispose();
            this.udpClient = null;
            return base.StopAsync(cancellationToken);
        }

        protected override void ThreadProc(CancellationToken cancellationToken)
        {
            while ((!cancellationToken.IsCancellationRequested) && (this.udpClient != null))
            {
                try
                {
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    var receivedBytes = this.udpClient.Receive(ref sender);
                    if (this.newThread)
                    {
                        this.HandleMessage(sender, receivedBytes);
                    }
                    else
                    {
                        new Thread(() =>
                        {
                            this.HandleMessage(sender, receivedBytes);
                        }).Start();
                    }
                }
                catch (SocketException)
                {
                    // this is fine, were shutting down
                    return;
                }
                Thread.Sleep(0);
            }
        }

        protected abstract void HandleMessage(IPEndPoint sender, byte[] receivedBytes);
    }
}