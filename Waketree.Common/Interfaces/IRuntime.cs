using Waketree.Common.Models;

namespace Waketree.Common.Interfaces

{
    public interface IRuntime
    {
        // Constructor should always take:
        //  runtimeString
        //  applicationDirectory
        //  ILogger<IRuntime>
        //  IServiceInterop

        string RuntimeString { get; }

        bool RunThread(WaketreeProcess process,
            WaketreeThread thread);

        void KillThread(string threadID);
    }
}
