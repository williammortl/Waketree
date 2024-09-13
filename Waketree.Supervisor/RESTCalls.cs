using Waketree.Common;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;

namespace Waketree.Supervisor
{
    static class RESTCalls
    {
        public static bool SupervisorConnect(string ipAddress)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<SupervisorConnectResponse>(
                string.Format(WaketreeConstants.URLServiceSupervisorConnect,
                    ipAddress,
                    Config.GetConfig().ServiceRESTPort)).Result;
            return response.Item1;
        }

        public static StatsResponse? GetServiceStats(string ipAddress)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<StatsResponse>(
                string.Format(WaketreeConstants.URLServiceStatus,
                    ipAddress,
                    Config.GetConfig().ServiceRESTPort)).Result;
            return (response.Item1) ? response.Item3 : null;
        }

        public static ThreadRunResponse? RunThread(WaketreeThread arg)
        {
            var response = RESTUtilities.PostCallRestServiceAsync<ThreadRunResponse, WaketreeThread>(
                string.Format(WaketreeConstants.URLServiceRunThread,
                    arg.IPAddress,
                    Config.GetConfig().ServiceRESTPort),
                arg).Result;
            return (response.Item1) ? response.Item3 : null;
        }

        public static ThreadKillResponse? KillThread(WaketreeThread threadToKill)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<ThreadKillResponse>(
                string.Format(WaketreeConstants.URLServiceStatus,
                    threadToKill.IPAddress,
                    Config.GetConfig().ServiceRESTPort,
                    threadToKill.ThreadID)).Result;
            return (response.Item1) ? response.Item3 : null;
        }
    }
}
