namespace Waketree.Common.Models
{
    public sealed class UDPMessage : ICloneable
    {
        public UDPMessageTopic Topic { get; set; }

        public string? Data { get; set; }

        public object Clone()
        {
            var retVal = new UDPMessage
            {
                Topic = this.Topic,
                Data = (this.Data == null) ?
                    null :
                    (string)this.Data.Clone()
            };
            return retVal;
        }
    }
}
