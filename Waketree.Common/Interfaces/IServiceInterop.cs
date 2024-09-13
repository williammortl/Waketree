namespace Waketree.Common.Interfaces

{
    public interface IServiceInterop
    {
        // Constructor should always take:
        //  IServiceMemory
        //  ILogger<IServiceInterop>

        IServiceMemory Memory { get; }

        void CreateWaketreeThread(string processID, string runtime, string? threadStartLocation, object? threadFrame);

        void WaketreeThreadEnded(string threadID);
    }
}
