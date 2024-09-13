using System.Collections.Concurrent;
using Waketree.Common.Models;

namespace Waketree.Common.Interfaces
{
    public interface ILoadBalancer
    {
        string WhichService(ConcurrentDictionary<string, StatsResponse> services, WaketreeThread newThread);
    }
}
