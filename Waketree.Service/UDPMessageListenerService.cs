using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Waketree.Common.Bases;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service
{
    sealed class UDPMessageListenerService : BaseUDPListenerService
    {
        private State state;
        private ILogger<UDPMessageListenerService> logger;

        public UDPMessageListenerService(ILogger<UDPMessageListenerService> logger) : 
            base(Config.GetConfig().CreateLogger<BaseUDPListenerService>(),
                new IPEndPoint(IPAddress.Any, Config.GetConfig().UDPPort),
                true)
        {
            this.state = State.GetState();
            this.logger = Config.GetConfig().CreateLogger<UDPMessageListenerService>();
        }

        protected override void HandleMessage(IPEndPoint sender, byte[] receivedBytes)
        {
            var receivedJSON = Encoding.ASCII.GetString(receivedBytes);
            this.logger.LogTrace(string.Format("UDP message from {0}:{1}\r\n{2}",
                sender.Address.ToString(),
                sender.Port,
                receivedJSON));
            try
            {
                try
                {
                    var udpMessage = JsonConvert.DeserializeObject<UDPMessage>(receivedJSON);
                    if (udpMessage != null)
                    {
                        this.state.UDPMessageQueue.Enqueue(new UDPMessageWrapper()
                        {
                            Message = udpMessage,
                            SenderIPAddress = sender.Address.ToString()
                        });
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogWarning(string.Format("Could not deserialize UDP message from {0}:{1}\r\n{2}\r\n{3}",
                        sender.Address.ToString(),
                        sender.Port,
                        receivedJSON,
                        e.ToString()));
                }
            }
            catch
            {
                this.logger.LogWarning(string.Format("BAD UDP message from {0}:{1}\r\n{2}",
                    sender.Address.ToString(),
                    sender.Port,
                    receivedJSON));
            }
        }
    }
}
