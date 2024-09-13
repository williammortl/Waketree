namespace Waketree.Common.Models
{
    public sealed class WaketreeProcess : ProcessCreateArgument
    {
        public string ProcessID { get; set; } 
        public DateTime Created {  get; set; }

        public WaketreeProcess()
        {
            this.Created = DateTime.Now;
        }

        public WaketreeProcess(ProcessCreateArgument arg)
        {
            this.ProcessID = Guid.NewGuid().ToString();
            this.Filename = arg.Filename;
            this.Runtime = arg.Runtime;
            this.CommandArgs = arg.CommandArgs;
        }
    }
}
