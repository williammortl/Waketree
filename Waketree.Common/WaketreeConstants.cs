namespace Waketree.Common
{
    public static class WaketreeConstants
    {
        // Service and Supervisor defaults

        public const string DefaultDllPath = "";

        // Service config defaults

        public const int DefaultServiceRESTPort = 1604;

        public const int DefaultServiceUDPPort = 1606;

        public const float DefaultAvailibilityScoreMemWeight = 0.34F;
        
        public const float DefaultAvailabilityScoreCPUWeight = 0.66F;

        public const float DefaultMemoryFloorPercent = 10;

        public const float DefaultCPUFloorPercent = 10;

        public const int DefaultMemoryCeilingMB = 16384;

        public const float DefaultCPUCeilingPercent = 80;

        public const int DefaultMaxSupervisorContactMinutes = 10;

        public const string DefaultApplicationPath = "";

        public const string DefaultServiceMemory = "Waketree.Service.dll,Waketree.Service.ConventionalServiceMemory";

        // Supervisor config defaults

        public const int DefaultSupervisorRESTPort = 1603;

        public const int DefaultSupervisorUDPPort = 1605;

        public const string DefaultLoadBalancer = "Waketree.Supervisor.dll,Waketree.Supervisor.RandomLoadBalancer";

        public const string DefaultSupervisorMemory = "Waketree.Supervisor.dll,Waketree.Supervisor.ConventionalSupervisorMemory";

        // URLs

        public const string URLServiceSupervisorConnect = "http://{0}:{1}/Supervisor/Connect";

        public const string URLServiceStatus = "http://{0}:{1}/Stats";

        public const string URLSupervisorGetProcess = "http://{0}:{1}/Process/{2}";

        public const string URLServiceRunThread = "http://{0}:{1}/Thread/Run";

        public const string URLServiceKillThread = "http://{0}:{1}/Thread/{2}/Kill";

        public const string URLServiceGetMemory = "http://{0}:{1}/Memory/{2}/{3}";

        public const string URLSupervisorGetMemory = "http://{0}:{1}/Memory/{2}/{3}";

        public const string URLSupervisorRegisterMemory = "http://{0}:{1}/Memory/Register";

        public const string URLServiceAddOrUpdate = "http://{0}:{1}/Memory/AddOrUpdate";

        public const string URLServiceTestAndSet = "http://{0}:{1}/Memory/TestAndSet";

        public const string URLSupervisorMemoryExistsProcess = "http://{0}:{1}/Memory/{2}/Exists";

        public const string URLSupervisorMemoryExists = "http://{0}:{1}/Memory/{2}/{3}/Exists";

        public const string URLServiceMemoryDelete = "http://{0}:{1}/Memory/{2}/{3}/Delete";

        public const string URLSupervisorMemoryDeregister = "http://{0}:{1}/Memory/{2}/{3}/Deregister";

        public const string URLSupervisorCreateThread = "http://{0}:{1}/Thread/Create";

        public const string URLSupervisorThreadEnded = "http://{0}:{1}/Thread/{2}/Ended";

        // Misc

        public const string SupervisorIPLookup = "127.0.0.1";
    }
}
