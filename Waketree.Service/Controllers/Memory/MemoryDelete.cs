using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers.Memory
{
    [ApiController]
    [Route("Memory/{processID}/{memoryName}/Delete")]
    public sealed class MemoryDelete : ControllerBase
    {

        private readonly ILogger<MemoryDelete> logger;

        public MemoryDelete(ILogger<MemoryDelete> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public MemoryDeleteResponse Get(string processID, string memoryName)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            State.GetState().Memory.DeleteMemory(processID, memoryName);

            return new MemoryDeleteResponse()
            {
                Success = true
            };
        }
    }
}
