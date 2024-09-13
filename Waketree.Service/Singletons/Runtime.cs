using System.Collections.Concurrent;
using Waketree.Common;
using Waketree.Common.Interfaces;

namespace Waketree.Service.Singletons
{
    public static class Runtime
    {
        private static ConcurrentDictionary<string, IRuntime> runtimeCache =
            new ConcurrentDictionary<string, IRuntime>();

        public static IRuntime GetRuntime(string runtimeString)
        {
            IRuntime runtime;
            if (!Runtime.runtimeCache.TryGetValue(runtimeString, out runtime))
            {
                var config = Config.GetConfig();
                var runtimeClass = runtimeString.Split(',');
                runtimeClass[0] = Path.Combine(config.DllPath, runtimeClass[0]);
                var newRuntime = Utils.LoadClass<IRuntime>(
                    runtimeClass,
                    new NullReferenceException(
                        string.Format("Could not find and load runtime: {0}", runtimeClass[1])),
                    new object[]
                    {
                        runtimeString,
                        config.ApplicationPath,
                        ServiceInterop.GetServiceInterop(),
                        config.CreateLogger<IRuntime>()
                    });
                runtime = (IRuntime)newRuntime;
                Runtime.runtimeCache.TryAdd(runtimeString, runtime);
            }
            return runtime;
        }
    }
}
