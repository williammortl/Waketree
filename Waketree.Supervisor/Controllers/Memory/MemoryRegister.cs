using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Memory/Register")]
    public sealed class MemoryRegister : ControllerBase
    {

        private readonly ILogger<MemoryRegister> logger;

        public MemoryRegister(ILogger<MemoryRegister> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public MemoryRegisterResponse Post(WaketreeMemory arg)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            arg.MemoryLocation = ip; // not sure if I need this
            State.GetState().Memory.RegisterMemory(arg);

            return new MemoryRegisterResponse()
            {
                Success = true
            };
        }
    }
}