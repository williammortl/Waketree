namespace Waketree.Common.Models
{
    public sealed class UDPMessageWrapper : ICloneable
    {
        public UDPMessage? Message { get; set; }
        public string SenderIPAddress { get; set; } = string.Empty;

        public object Clone()
        {
            return new UDPMessageWrapper
            {
                Message = (this.Message == null) ?
                    null :
                    (UDPMessage)this.Message.Clone(),
                SenderIPAddress = (string)this.SenderIPAddress.Clone()
            };
        }
    }
}
