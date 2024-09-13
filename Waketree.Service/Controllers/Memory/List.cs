using Microsoft.AspNetCore.Mvc;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers.Memory
{

    [ApiController]
    [Route("Memory")]
    public sealed class List : ControllerBase
    {

        private readonly ILogger<List> logger;

        public List(ILogger<List> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<Tuple<string, string>> Get()
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return State.GetState().Memory.LocalMemoryInfo;
        }
    }
}