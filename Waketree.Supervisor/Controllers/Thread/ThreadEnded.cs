using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Thread/{threadID}/Ended")]
    public sealed class ThreadEnded : ControllerBase
    {

        private readonly ILogger<ThreadEnded> logger;

        public ThreadEnded(ILogger<ThreadEnded> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public ThreadEndedResponse Get(string threadID)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            State.GetState().OperationQueue.Enqueue(new Models.SupervisorOperation()
            {
                Operation = SupervisorOperations.ThreadEnded,
                SenderIPAddress = ip,
                Data = threadID
            });

            return new ThreadEndedResponse()
            {
                Success = true
            };
        }
    }
}