using Microsoft.AspNetCore.Mvc;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Memory
{
    [ApiController]
    [Route("Memory/{processID}/Exists")]
    public sealed class MemoryExistsProcess : ControllerBase
    {

        private readonly ILogger<MemoryExistsProcess> logger;

        public MemoryExistsProcess(ILogger<MemoryExistsProcess> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public bool Get(string processID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            var retVal = State.GetState().Memory[processID];
            return ((retVal != null) && (retVal.Count() > 0)) ?
                true :
                false;
        }
    }
}
