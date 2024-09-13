namespace Waketree.Common.Models
{
    public sealed class ThreadCreateResponse
    {
        public bool Success { get; set; }
        public string ThreadID { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
    }
}
