using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor.Controllers.Process
{
    [ApiController]
    [Route("Process/Create")]
    public sealed class ProcessCreate : ControllerBase
    {

        private readonly ILogger<ProcessCreate> logger;

        public ProcessCreate(ILogger<ProcessCreate> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public ProcessCreateResponse Post(ProcessCreateArgument arg)
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            var newProcess = new WaketreeProcess(arg)
            {
                ProcessID = Guid.NewGuid().ToString()
            };
            State.GetState().OperationQueue.Enqueue(new Models.SupervisorOperation()
            {
                Operation = SupervisorOperations.CreateProcess,
                SenderIPAddress = ip,
                Data = newProcess
            });

            return new ProcessCreateResponse()
            {
                Success = true,
                ProcessID = newProcess.ProcessID
            };
        }
    }
}
