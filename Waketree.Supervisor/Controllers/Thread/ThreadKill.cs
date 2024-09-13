using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Thread/{threadID}/Kill")]
    public sealed class ThreadKill : ControllerBase
    {

        private readonly ILogger<ThreadKill> logger;

        public ThreadKill(ILogger<ThreadKill> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ThreadKillResponse Get(string threadID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            State.GetState().OperationQueue.Enqueue(new Models.SupervisorOperation()
            {
                Operation = SupervisorOperations.KillThread,
                SenderIPAddress = ip,
                Data = threadID
            });

            return new ThreadKillResponse()
            {
                Success = true
            };
        }
    }
}