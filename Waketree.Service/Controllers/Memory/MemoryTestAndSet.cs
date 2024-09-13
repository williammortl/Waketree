using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers.Memory
{
    [ApiController]
    [Route("Memory/TestAndSet")]
    public sealed class MemoryTestAndSet : Controller
    {

        private readonly ILogger<MemoryTestAndSet> logger;

        public MemoryTestAndSet(ILogger<MemoryTestAndSet> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public TestAndSetResponse Post(WaketreeMemoryTestAndSet arg)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            var retVal = new TestAndSetResponse()
            {
                Success = State.GetState().Memory.TestAndSet(arg.ProcessID, arg.MemoryName, arg.MemoryValue?.DeserializeObject(), arg.MemoryExpectedValue?.DeserializeObject())
            };
            return retVal;
        }
    }
}
