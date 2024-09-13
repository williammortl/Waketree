using Waketree.Common.Interfaces;
using Waketree.Common.Models;

namespace Waketree.Service.Singletons
{
    public class ServiceInterop : IServiceInterop
    {
        private static IServiceInterop? instance;

        private ILogger<IServiceInterop> logger;
        private State state;

        private ServiceInterop()
        {
            var config = Config.GetConfig();
            this.logger = config.CreateLogger<IServiceInterop>();
            this.state = State.GetState();
        }

        public IServiceMemory Memory
        {
            get
            {
                return this.state.Memory;
            }
        }

        public static IServiceInterop GetServiceInterop()
        {
            if (ServiceInterop.instance == null)
            {
                ServiceInterop.instance = new ServiceInterop();
            }
            return ServiceInterop.instance;
        }

        public void CreateWaketreeThread(string processID, string runtime, string? threadStartLocation, object? threadFrame)
        {
            RESTCalls.CreateThread(new ThreadCreateArgument()
            {
                ProcessID = processID,
                ThreadStartLocation = threadStartLocation,
                ThreadFrame = (threadFrame == null) ? null : new ComplexSerializableObject(threadFrame),
                Runtime = runtime
            });
        }

        public void WaketreeThreadEnded(string threadID)
        {
            RESTCalls.ThreadEnded(threadID);
        }
    }
}
