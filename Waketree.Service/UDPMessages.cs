using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Waketree.Common.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service
{
    public static class UDPMessages
    {
        public static void BroadcastWhoIsTheSupervisor()
        {
            var message = new UDPMessage()
            {
                Topic = Common.UDPMessageTopic.SupervisorQuery
            };
            using (var udpClient = new UdpClient())
            {
                var messageString = JsonConvert.SerializeObject(message);
                var messageData = Encoding.UTF8.GetBytes(messageString);
                udpClient.Send(messageData, messageData.Length, 
                    new IPEndPoint(IPAddress.Broadcast, 
                    Config.GetConfig().SupervisorUDPPort));
            }
        }

        public static void ServiceDisconnecting(string ipAddress)
        {
            var message = new UDPMessage()
            {
                Topic = Common.UDPMessageTopic.ServiceDisconnecting
            };
            using (var udpClient = new UdpClient())
            {
                var messageString = JsonConvert.SerializeObject(message);
                var messageData = Encoding.UTF8.GetBytes(messageString);
                udpClient.Send(messageData, messageData.Length,
                    new IPEndPoint(IPAddress.Parse(ipAddress),
                    Config.GetConfig().SupervisorUDPPort));
            }
        }
    }
}
