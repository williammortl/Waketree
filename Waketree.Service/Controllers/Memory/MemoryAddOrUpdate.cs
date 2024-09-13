using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Service.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers.Memory
{
    [ApiController]
    [Route("Memory/AddOrUpdate")]
    public sealed class MemoryAddOrUpdate : Controller
    {

        private readonly ILogger<MemoryAddOrUpdate> logger;

        public MemoryAddOrUpdate(ILogger<MemoryAddOrUpdate> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public MemoryUpdateResponse Post(WaketreeMemoryValue arg)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            State.GetState().Memory.AddOrUpdateMemory(arg.ProcessID, arg.MemoryName, arg.MemoryValue?.DeserializeObject());

            return new MemoryUpdateResponse()
            {
                Success = true
            };
        }
    }
}
