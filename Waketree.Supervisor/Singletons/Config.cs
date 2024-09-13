using Waketree.Common;

namespace Waketree.Supervisor.Singletons
{
    sealed class Config
    {
        private const string JsonConfigSection = "Waketree.Supervisor";
        private const string JsonConfigQuery = JsonConfigSection + ":{0}";
        private const string JsonKeyUDPPort = "UDPPort";
        private const string JsonKeyServiceRESTPort = "ServiceRESTPort";
        private const string JsonKeyServiceUDPPort = "ServiceUDPPort";
        private const string JsonKeyLoadBalancer = "LoadBalancer";
        private const string JsonKeySupervisorMemory = "SupervisorMemory";
        private const string JsonKeyDLLPath = "DllPath";

        private static Config? instance;

        private string loadBalancer;
        private string supervisorMemory;

        public IConfiguration AppsettingsConfig { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public int UDPPort { get; private set; }
        public int ServiceRESTPort { get; private set; }
        public int ServiceUDPPort { get; private set; }
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

            this.UDPPort = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyUDPPort));
            this.UDPPort = this.UDPPort == 0 ?
                WaketreeConstants.DefaultSupervisorUDPPort : this.UDPPort;
             
            this.ServiceRESTPort = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyServiceRESTPort));
            this.ServiceRESTPort = this.ServiceRESTPort == 0 ?
                WaketreeConstants.DefaultServiceRESTPort : this.ServiceRESTPort;

            this.ServiceUDPPort = this.AppsettingsConfig.GetValue<int>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyServiceUDPPort));
            this.ServiceUDPPort = this.ServiceUDPPort == 0 ?
                WaketreeConstants.DefaultServiceUDPPort : this.ServiceUDPPort;

            var loadBalancer = this.AppsettingsConfig.GetValue<string>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeyLoadBalancer));
            this.loadBalancer = (loadBalancer == null) ?
                WaketreeConstants.DefaultLoadBalancer : loadBalancer;

            var supervisorMemory = this.AppsettingsConfig.GetValue<string>(
                string.Format(Config.JsonConfigQuery, Config.JsonKeySupervisorMemory));
            this.supervisorMemory = (supervisorMemory == null) ?
                WaketreeConstants.DefaultSupervisorMemory : supervisorMemory;

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

        public string[] LoadBalancer
        {
            get
            {
                var split = this.loadBalancer.Split(',');
                split[0] = Path.Combine(this.DllPath, split[0]);
                return split;
            }
        }

        public string[] SupervisorMemory
        {
            get
            {
                var split = this.supervisorMemory.Split(',');
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
