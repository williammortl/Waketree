using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{

    [ApiController]
    [Route("Process/{processID}")]
    public sealed class Process : Controller
    {

        private readonly ILogger<Process> logger;

        public Process(ILogger<Process> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public WaketreeProcess? Get(string processID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return State.GetState()?.GetProcessByID(processID);
        }
    }
}
