using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{

    [ApiController]
    [Route("Memory/{processID}/{memoryName}")]
    public sealed class Memory : Controller
    {

        private readonly ILogger<Memory> logger;

        public Memory(ILogger<Memory> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public WaketreeMemory? Get(string processID, string memoryName)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return State.GetState().Memory.GetMemory(processID, memoryName);
        }
    }
}
