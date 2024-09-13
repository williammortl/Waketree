using Microsoft.AspNetCore.Mvc;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Variable
{

    [ApiController]
    [Route("Memory/{processID}")]
    public sealed class ListByProcess : ControllerBase
    {

        private readonly ILogger<ListByProcess> logger;

        public ListByProcess(ILogger<ListByProcess> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get(string processID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return State.GetState().Memory[processID];
        }
    }
}
