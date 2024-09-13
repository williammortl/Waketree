namespace Waketree.Neptune.Common.Exceptions
{
    public class InvalidOpCodeArgumentException : Exception
    {
        public InvalidOpCodeArgumentException(string message) : base(message) { }

        public InvalidOpCodeArgumentException(string message, Exception innerExcetion) : base(message, innerExcetion) { }
    }
}
