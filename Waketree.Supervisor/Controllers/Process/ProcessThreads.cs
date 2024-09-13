using Microsoft.AspNetCore.Mvc;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Process/{processID}/Threads")]
    public sealed class ProcessThreads : ControllerBase
    {

        private readonly ILogger<ProcessThreads> logger;

        public ProcessThreads(ILogger<ProcessThreads> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IEnumerable<string> Get(string processID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            var retVal = State.GetState().GetThreadsByProcessID(processID)?.Select(s => s.ThreadID);
            retVal = (retVal == null) ? 
                new List<string>() :
                retVal;

            return retVal;
        }
    }
}