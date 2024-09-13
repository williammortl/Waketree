using Microsoft.AspNetCore.Mvc;
using Waketree.Common;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service.Controllers
{

    [ApiController]
    [Route("Stats")]
    public sealed class Stats : Controller
    {

        private readonly ILogger<Stats> logger;
        private Config config;
        private State state;

        public Stats(ILogger<Stats> logger)
        {
            this.logger = logger;
            this.config = Config.GetConfig();
            this.state = State.GetState();
        }

        [HttpGet]
        public StatsResponse Get()
        {
            var ip = "{UNKNOWN}";
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            this.logger.LogTrace(string.Format("Queried from {0}", ip));

            // if supervisor, record that they talked to us!
            if (ip == this.state.SupervisorIP)
            {
                this.state.TalkedToSupervisor();
            }

            var totalMemoryMB = OSUtilities.GetTotalMemoryMB();
            var memoryFreeMB = OSUtilities.GetMemoryFreeMB();
            var memoryFreePercent = 100 * ((float)memoryFreeMB / (float)totalMemoryMB);
            var memoryFreeMBScore = (memoryFreeMB > config.MemoryCeilingMB) ?
                100 :
                memoryFreePercent; 
            var cpuFreePercent = 100 - OSUtilities.GetCPUUtilizationPercent();
            var cpuFreeScore = (cpuFreePercent > config.CPUCeilingPercent) ? 100 : cpuFreePercent;
            var memoryWeightedScore = (memoryFreeMBScore >= config.MemoryFloorPercent) ?
                config.AvailabilityMemWeight * memoryFreeMBScore :
                0;
            var cpuWeightedScore = (cpuFreeScore >= config.CPUFloorPercent) ?
                config.AvailabilityCPUWeight * cpuFreeScore :
                0;
            return new StatsResponse()
            {
                State = this.state.ServiceState,
                ComputerName = OSUtilities.GetComputerName(),
                OSPlatform = OSUtilities.GetOSPlatform(),
                OSVersion = OSUtilities.GetOSVersion(),
                CpuCores = OSUtilities.GetCPUCores(),
                CpuFreePercent = cpuFreePercent,
                TotalMemoryMB = totalMemoryMB,
                MemoryFreeMB = memoryFreeMB,
                MemoryFreePercent = memoryFreePercent,
                AvailabilityScore = ((cpuWeightedScore > 0) && (memoryWeightedScore > 0)) ?
                    cpuWeightedScore + memoryWeightedScore :
                    0,
                SupervisorIP = this.state.SupervisorIP,
                Memory = this.config.ServiceMemory[0] + "," + this.config.ServiceMemory[1],
                Runtimes = this.config.Runtimes,
                ProcessCount = this.state.ProcessesList.Count(),
                ThreadCount = this.state.ThreadsList.Count(),
                VariableCount = 1604 // TODO
            };
        }
    }
}
