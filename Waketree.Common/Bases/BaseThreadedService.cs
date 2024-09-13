using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Waketree.Common.Bases
{
    public abstract class BaseThreadedService : IHostedService
    {
        private CancellationTokenSource cancellationTokenSource;
        private Thread? thread;

        public BaseThreadedService()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.thread != null)
            {
                return Task.CompletedTask;
            }
            this.cancellationTokenSource = new CancellationTokenSource();
            this.thread = new Thread(() =>
            {
                this.ThreadProc(this.cancellationTokenSource.Token);
            });
            this.thread.Start();
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            if (this.thread == null)
            {
                return Task.CompletedTask;
            }
            this.cancellationTokenSource?.Cancel();
            Thread.Sleep(0);
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = null;
            this.thread = null;
            return Task.CompletedTask;
        }

        protected abstract void ThreadProc(CancellationToken cancellationToken);
    }
}
