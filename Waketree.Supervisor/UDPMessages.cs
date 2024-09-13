using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Waketree.Common.Models;
using Waketree.Supervisor.Singletons;
using System.Runtime.CompilerServices;

namespace Waketree.Supervisor
{
    static class UDPMessages
    {
        public static void BroadcastWhoAreServices()
        {
            var message = new UDPMessage()
            {
                Topic = Common.UDPMessageTopic.ServiceQuery
            };
            UDPMessages.BroadcastMessage(message, Config.GetConfig().ServiceUDPPort);
        }

        public static void BroadcastShutdownToServices()
        {
            var message = new UDPMessage()
            {
                Topic = Common.UDPMessageTopic.Shutdown
            };
            UDPMessages.BroadcastMessage(message, Config.GetConfig().ServiceUDPPort);
        }

        public static void BroadcastKillProcess(string processID)
        {
            var message = new UDPMessage()
            {
                Topic = Common.UDPMessageTopic.KillProcess,
                Data = processID
            };
            UDPMessages.BroadcastMessage(message, Config.GetConfig().ServiceUDPPort);
        }

        private static void BroadcastMessage(UDPMessage message, int port)
        {
            using (var udpClient = new UdpClient())
            {
                var messageString = JsonConvert.SerializeObject(message);
                var messageData = Encoding.UTF8.GetBytes(messageString);
                udpClient.Send(messageData, messageData.Length,
                    new IPEndPoint(IPAddress.Broadcast,
                    port));
            }
        }
    }
}
