using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Waketree.Common
{
    public static class Utils
    {
        public static T LoadClass<T>(string[] dllAndClass, Exception eToThrow)
        {
            var assembly = Assembly.LoadFrom(dllAndClass[0]);
            var type = assembly.GetType(dllAndClass[1]);
            if (type == null)
            {
                throw eToThrow;
            }
            var newInstance = Activator.CreateInstance(type);
            if (newInstance == null)
            {
                throw eToThrow;
            }
            return (T)newInstance;
        }

        public static T LoadClass<T>(string[] dllAndClass, Exception eToThrow, params object[] constructorArgs)
        {
            var assembly = Assembly.LoadFrom(dllAndClass[0]);
            var type = assembly.GetType(dllAndClass[1]);
            if (type == null)
            {
                throw eToThrow;
            }
            var newInstance = Activator.CreateInstance(type, constructorArgs);
            if (newInstance == null)
            {
                throw eToThrow;
            }
            return (T)newInstance;
        }

        public static string GetPrimaryIP6()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName, AddressFamily.InterNetworkV6);
            return addresses[0].ToString();
        }

        public static string GetPrimaryIP4()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName, AddressFamily.InterNetwork);
            return addresses[0].ToString();
        }
    }
}
