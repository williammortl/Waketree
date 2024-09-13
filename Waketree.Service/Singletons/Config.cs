using Waketree.Common;

namespace Waketree.Service.Singletons
{
    sealed class Config
    {
        private const string JsonConfigSection = "Waketree.Service";
        private const string JsonConfigQuery = JsonConfigSection + ":{0}";
        private const string JsonKeyUDPPort = "UDPPort";
        private const string JsonKeyAvailabilityMemWeight = "AvailabilityMemWeight";
        private const string JsonKeyAvailabilityCPUWeight = "AvailabilityCPUWeight";
        private const string JsonKeyMemoryFloorPercent = "MemoryFloorPercent";
        private const string JsonKeyCPUFloorPercent = "CPUFloorPercent";
        private const string JsonKeyMemoryCeilingMB = "MemoryCeilingMB";
        private const string JsonKeyCPUCeilingPercent = "CPUCeilingPercent";
        private const string JsonKeyMaxSupervisorContactMinutes = "MaxSupervisorContactMinutes";
        private const string JsonKeySupervisorRESTPort = "SupervisorRESTPort";
        private const string JsonKeySupervisorUDPPort = "SupervisorUDPPort";
        private const string JsonKeyServiceRESTPort = "ServiceRESTPort";
        private const string JsonKeyServiceMemory = "ServiceMemory";
        private const string JsonKeApplicationPath = "ApplicationPath";
        private const string JsonSectionRuntimes = "Runtimes";       
        private const string JsonKeyDLLPath = "DllPath";


        private static Config? instance;

        private string serviceMemory;

        public IConfiguration AppsettingsConfig { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public int UDPPort { get; private set; }
        public float AvailabilityMemWeight { get; private set; }
        public float AvailabilityCPUWeight { get; private set; }
        public float MemoryFloorPercent { get; private set; }
        public float CPUFloorPercent { get; private set; }
        public int MemoryCeilingMB { get; private set; }
        public float CPUCeilingPercent { get; private set; }
        public int MaxSupervisorContactMinutes { get; private set; }
        public int SupervisorRESTPort { get; private set; }
        public int SupervisorUDPPort { get; private set; }
        public int ServiceRESTPort { get; private set; }
        public string ApplicationPath { get; private set; }
        public string[] Runtimes { get; private set; }
        public string DllPath { get; private set; }

        private Config()
        {
            this.AppsettingsConfig = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            this.LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConfiguration(AppsettingsConfig.GetSection("Logging"));
                builder.AddConsole();
            });

            // load and set default if empty
            this.UDPPort = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyUDPPort));
            this.UDPPort = this.UDPPort == 0 ?
                WaketreeConstants.DefaultServiceUDPPort : this.UDPPort;

            this.AvailabilityMemWeight = this.AppsettingsConfig.GetValue<float>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyAvailabilityMemWeight));
            this.AvailabilityMemWeight = this.AvailabilityMemWeight == 0F ?
                WaketreeConstants.DefaultAvailibilityScoreMemWeight : this.AvailabilityMemWeight;

            this.AvailabilityCPUWeight = this.AppsettingsConfig.GetValue<float>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyAvailabilityCPUWeight));
            this.AvailabilityCPUWeight = this.AvailabilityCPUWeight == 0F ?
                WaketreeConstants.DefaultAvailabilityScoreCPUWeight : this.AvailabilityCPUWeight;

            this.MemoryFloorPercent = this.AppsettingsConfig.GetValue<float>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyMemoryFloorPercent));
            this.MemoryFloorPercent = this.MemoryFloorPercent == 0F ?
                WaketreeConstants.DefaultMemoryFloorPercent : this.MemoryFloorPercent;

            this.CPUFloorPercent = this.AppsettingsConfig.GetValue<float>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyCPUFloorPercent));
            this.CPUFloorPercent = this.CPUFloorPercent == 0F ?
                WaketreeConstants.DefaultCPUFloorPercent : this.CPUFloorPercent;

            this.MemoryCeilingMB = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyMemoryCeilingMB));
            this.MemoryCeilingMB = this.MemoryCeilingMB == 0 ?
                WaketreeConstants.DefaultMemoryCeilingMB : this.MemoryCeilingMB;

            this.CPUCeilingPercent = this.AppsettingsConfig.GetValue<float>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyCPUCeilingPercent));
            this.CPUCeilingPercent = this.CPUCeilingPercent == 0F ?
                WaketreeConstants.DefaultCPUCeilingPercent : this.CPUCeilingPercent;

            this.MaxSupervisorContactMinutes = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyMaxSupervisorContactMinutes));
            this.MaxSupervisorContactMinutes = this.MaxSupervisorContactMinutes == 0 ?
                WaketreeConstants.DefaultMaxSupervisorContactMinutes : this.MaxSupervisorContactMinutes;

            this.SupervisorRESTPort = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeySupervisorRESTPort));
            this.SupervisorRESTPort = this.SupervisorRESTPort == 0 ?
                WaketreeConstants.DefaultSupervisorRESTPort : this.SupervisorRESTPort;

            this.SupervisorUDPPort = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeySupervisorUDPPort));
            this.SupervisorUDPPort = this.SupervisorUDPPort == 0 ?
                WaketreeConstants.DefaultSupervisorUDPPort : this.SupervisorUDPPort;

            this.ServiceRESTPort = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyServiceRESTPort));
            this.ServiceRESTPort = this.ServiceRESTPort == 0 ?
                WaketreeConstants.DefaultServiceRESTPort : this.ServiceRESTPort;

            var serviceMemory = this.AppsettingsConfig.GetValue<string>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyServiceMemory));
            this.serviceMemory = (serviceMemory == null) ?
                WaketreeConstants.DefaultServiceMemory : serviceMemory;

            var applicationPath = this.AppsettingsConfig.GetValue<string>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeApplicationPath));
            this.ApplicationPath = (applicationPath == null) ?
                WaketreeConstants.DefaultApplicationPath :
                applicationPath;

            var runtimes = this.AppsettingsConfig.GetRequiredSection(
                string.Format(Config.JsonConfigQuery, Config.JsonSectionRuntimes)).Get<string[]>();
            if (runtimes == null)
            {
                throw new NullReferenceException("Either memory or runtime configuration was not present.");
            }
            this.Runtimes = runtimes;

            var dllPath = this.AppsettingsConfig.GetValue<string>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyDLLPath));
            this.DllPath = (dllPath == null) ?
                WaketreeConstants.DefaultDllPath : dllPath;
        }

        public static Config GetConfig()
        {
            if (Config.instance == null)
            {
                Config.instance = new Config();
            }
            return Config.instance;
        }

        public string[] ServiceMemory
        {
            get
            {
                var split = this.serviceMemory.Split(',');
                split[0] = Path.Combine(this.DllPath, split[0]);
                return split;
            }
        }

        public ILogger<T> CreateLogger<T>()
        {
            return this.LoggerFactory.CreateLogger<T>();
        }
    }
}
