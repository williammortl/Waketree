using System.Text;

namespace Waketree.Neptune.Common
{
    public static class Utils
    {
        public static byte[] DoubleToBytes(double value)
        {
            return BitConverter.GetBytes(value);
        }

        public static double BytesToDouble(byte[] bytes)
        {
            return Utils.BytesToDouble(bytes, 0);
        }

        public static double BytesToDouble(byte[] bytes, int start)
        {
            return BitConverter.ToDouble(bytes, start);
        }

        public static byte[] LongToBytes(long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static long BytesToLong(byte[] bytes)
        {
            return Utils.BytesToLong(bytes, 0);
        }

        public static long BytesToLong(byte[] bytes, int start)
        {
            return BitConverter.ToInt64(bytes, start);
        }

        public static byte[] StringToNeptuneStringBytes(string value)
        {
            var stringBytes = Encoding.UTF8.GetBytes(value).ToList<byte>();
            var bytes = Utils.LongToBytes((long)stringBytes.Count).ToList<byte>();
            bytes.AddRange(stringBytes);
            return bytes.ToArray<byte>();
        }

        public static string NeptuneStringBytesToString(byte[] value)
        {

            return Utils.NeptuneStringBytesToString(value, 0);
        }

        public static string NeptuneStringBytesToString(byte[] value, int start)
        {
            var stringLen = Utils.BytesToLong(value, start);
            return Encoding.UTF8.GetString(value, start + 8, (int)stringLen);
        }
    }
}
