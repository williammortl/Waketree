using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers.Memory
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
        public ComplexSerializableObject? Get(string processID, string memoryName)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            var memVal = State.GetState().Memory.GetMemory(processID, memoryName);

            return (memVal == null) ?
                null :
                new ComplexSerializableObject(memVal);
        }
    }
}
