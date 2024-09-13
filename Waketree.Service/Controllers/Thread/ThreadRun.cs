using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Service;
using Waketree.Service.Models;
using Waketree.Service.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Thread/Run")]
    public sealed class ThreadRun : ControllerBase
    {

        private readonly ILogger<ThreadRun> logger;

        public ThreadRun(ILogger<ThreadRun> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public ThreadRunResponse Post(ThreadRunArgument arg)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            var operation = new ServiceOperation()
            {
                Operation = ServiceOperations.RunThread,
                SenderIPAddress = ip,
                Data = arg
            };
            State.GetState().OperationQueue.Enqueue(operation);

            return new ThreadRunResponse()
            {
                Success = true
            };
        }
    }
}