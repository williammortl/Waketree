using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;

namespace Waketree.Supervisor.Controllers
{

    [ApiController]
    [Route("Ping")]
    public sealed class Ping : ControllerBase
    {

        private readonly ILogger<Ping> logger;

        public Ping(ILogger<Ping> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public PingResponse Get()
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));
            return new PingResponse();
        }
    }
}
