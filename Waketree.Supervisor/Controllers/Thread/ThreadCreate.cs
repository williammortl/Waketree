using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Thread/Create")]
    public sealed class ThreadCreate : ControllerBase
    {

        private readonly ILogger<ThreadCreate> logger;

        public ThreadCreate(ILogger<ThreadCreate> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public ThreadCreateResponse Post(ThreadCreateArgument arg)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            var newThread = new WaketreeThread(arg)
            {
                ThreadID = Guid.NewGuid().ToString()
            };
            State.GetState().OperationQueue.Enqueue(new Models.SupervisorOperation()
            {
                Operation = SupervisorOperations.CreateThread,
                SenderIPAddress = ip,
                Data = newThread
            });

            return new ThreadCreateResponse()
            {
                Success = true,
                ThreadID = newThread.ThreadID
            };
        }
    }
}