namespace Waketree.Service.Models
{
    public class ServiceOperation
    {
        public ServiceOperations Operation { get; set; }
        public object? Data { get; set; }
        public string SenderIPAddress { get; set; } = string.Empty;
    }
}
