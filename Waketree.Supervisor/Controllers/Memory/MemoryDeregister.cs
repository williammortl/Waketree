using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Memory
{
    [ApiController]
    [Route("Memory/{processID}/{memoryName}/Deregister")]
    public sealed class MemoryDeregister : ControllerBase
    {

        private readonly ILogger<MemoryDeregister> logger;

        public MemoryDeregister(ILogger<MemoryDeregister> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public MemoryDeregisterResponse Get(string processID, string memoryName)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            State.GetState().Memory.DeregisterMemory(processID, memoryName);

            return new MemoryDeregisterResponse()
            {
                Success = true
            };
        }
    }
}
