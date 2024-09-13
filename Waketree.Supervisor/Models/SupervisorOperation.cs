namespace Waketree.Supervisor.Models
{
    sealed class SupervisorOperation
    {
        public SupervisorOperations Operation { get; set; }
        public string SenderIPAddress { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
