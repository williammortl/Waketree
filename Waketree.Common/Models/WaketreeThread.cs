namespace Waketree.Common.Models
{
    public class WaketreeThread : ThreadCreateArgument
    {
        public string IPAddress { get; set; }
        public DateTime Created { get; set; }
        public string ThreadID { get; set; }

        public WaketreeThread()
        { 
            this.Created = DateTime.Now;
        }

        public WaketreeThread(ThreadCreateArgument arg) :
            this()
        {
            this.ProcessID = arg.ProcessID;
            this.ThreadStartLocation = arg.ThreadStartLocation;
            this.ThreadFrame = arg.ThreadFrame; 
            this.Runtime = arg.Runtime;
        }  
    }
}
