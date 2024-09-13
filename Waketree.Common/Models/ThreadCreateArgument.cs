namespace Waketree.Common.Models
{
    public class ThreadCreateArgument
    {
        public string ProcessID { get; set; }
        public string? ThreadStartLocation { get; set; } // if this is null it means start the first thread for the process
        public ComplexSerializableObject? ThreadFrame { get; set; }
        public string Runtime { get; set; }
    }
}
