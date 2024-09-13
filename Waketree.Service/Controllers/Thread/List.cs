using Microsoft.AspNetCore.Mvc;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers.Thread
{
    [ApiController]
    [Route("Thread")]
    public sealed class List : ControllerBase
    {

        private readonly ILogger<List> logger;

        public List(ILogger<List> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            return State.GetState().ThreadsList.Select(t => t.ThreadID);
        }
    }
}
