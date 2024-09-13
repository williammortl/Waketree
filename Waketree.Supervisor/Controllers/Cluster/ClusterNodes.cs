using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Cluster
{
    [ApiController]
    [Route("Cluster/Nodes")]
    public sealed class ClusterNodes : ControllerBase
    {

        private readonly ILogger<ClusterNodes> logger;

        public ClusterNodes(ILogger<ClusterNodes> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ConcurrentDictionary<string, StatsResponse> Get()
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return State.GetState().StatsServices;
        }
    }
}
