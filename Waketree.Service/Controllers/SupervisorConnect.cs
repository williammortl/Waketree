using Microsoft.AspNetCore.Mvc;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers
{

    [ApiController]
    [Route("Supervisor/Connect")]
    public class SupervisorConnect : Controller
    {

        private readonly ILogger<SupervisorConnect> logger;
        private State state = State.GetState();

        public SupervisorConnect(ILogger<SupervisorConnect> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public SupervisorConnectResponse Get()
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            var retVal = new SupervisorConnectResponse();
            if (ip == "{UNKNOW}")
            {
                this.logger.LogWarning("Supervisor connected from indeterminate IP");
                retVal.ConnectionAcknowledged = false;
                this.state.SupervisorIP = string.Empty;
                this.state.ServiceState = Common.ServiceStates.Up;
            }
            else
            {
                this.logger.LogInformation(string.Format("Supervisor connected from IP: {0}", ip));
                retVal.ConnectionAcknowledged = true;
                State.GetState().SupervisorIP = ip;
                this.state.TalkedToSupervisor();
                this.state.ServiceState = Common.ServiceStates.Connected;
            }
            return retVal;
        }
    }
}
