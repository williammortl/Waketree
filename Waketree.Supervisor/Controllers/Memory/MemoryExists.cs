using Microsoft.AspNetCore.Mvc;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Memory
{
    [ApiController]
    [Route("Memory/{processID}/{memoryName}/Exists")]
    public sealed class MemoryExists : Controller
    {

        private readonly ILogger<MemoryExists> logger;

        public MemoryExists(ILogger<MemoryExists> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public bool Get(string processID, string memoryName)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return (State.GetState().Memory.GetMemory(processID, memoryName) != null) ?
                true :
                false;  
        }
    }
}
