namespace Waketree.Common.Models
{
    public sealed class PingResponse
    {
        public PingResponse()
        {
            this.Ping = "pong";
        }

        public string Ping { get; set; }
    }
}
