namespace Waketree.Common.Models
{
    public sealed class ThreadRunArgument : WaketreeThread
    { 
        public ThreadRunArgument()
        { }

        public ThreadRunArgument(WaketreeThread thread)
        {
            this.IPAddress = thread.IPAddress;
            this.Created = thread.Created;
            this.ThreadID = thread.ThreadID;
            this.ProcessID = thread.ProcessID;
            this.ThreadStartLocation = thread.ThreadStartLocation;
            this.ThreadFrame = thread.ThreadFrame;
            this.Runtime = thread.Runtime;
        }
    }
}
