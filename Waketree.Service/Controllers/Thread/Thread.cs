using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers.Thread
{
    [ApiController]
    [Route("Thread/{threadID}")]
    public class Thread : Controller
    {

        private readonly ILogger<Thread> logger;

        public Thread(ILogger<Thread> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public WaketreeThread? Get(string threadID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return State.GetState()?.GetThreadByID(threadID);
        }
    }
}
