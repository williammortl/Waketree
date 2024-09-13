using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Process/{processID}/Kill")]
    public sealed class ProcessKill : ControllerBase
    {

        private readonly ILogger<ProcessKill> logger;

        public ProcessKill(ILogger<ProcessKill> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ProcessKillResponse Get(string processID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            State.GetState().OperationQueue.Enqueue(new Models.SupervisorOperation()
            {
                Operation = SupervisorOperations.KillProcess,
                SenderIPAddress = ip,
                Data = processID
            });

            return new ProcessKillResponse()
            {
                Success = true
            };
        }
    }
}
