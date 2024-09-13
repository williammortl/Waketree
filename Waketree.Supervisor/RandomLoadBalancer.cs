using System.Collections.Concurrent;
using Waketree.Common.Models;
using Waketree.Common.Interfaces;

namespace Waketree.Supervisor
{
    sealed class RandomLoadBalancer : ILoadBalancer
    {
        private const int MaxRetries = 100;

        private Random random;

        public RandomLoadBalancer()
        {
            this.random = new Random();
        }

        public string WhichService(ConcurrentDictionary<string, StatsResponse> services, WaketreeThread newThread)
        {
            var retryCounter = 0;
            string selectedServiceIP = string.Empty;

            while ((selectedServiceIP == string.Empty) && (retryCounter < RandomLoadBalancer.MaxRetries))
            {
                var randomNum = this.random.Next(0, services.Count);
                var randomServiceIP = services.Keys.AsEnumerable<string>().ElementAt<string>(randomNum);
                var randomService = services[randomServiceIP];
                if (randomService.Runtimes.Contains(newThread.Runtime))
                {
                    selectedServiceIP = randomServiceIP;
                    newThread.IPAddress = selectedServiceIP;
                    break;
                }
                retryCounter++;
            }
            if (selectedServiceIP == string.Empty)
            {
                throw new NotSupportedException(
                    string.Format("Could not find a suitable service to execute the bytecode runtime: {0}",
                    newThread.Runtime));
            }
            return selectedServiceIP;
        }
    }
}
