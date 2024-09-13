using Waketree.Neptune.Common.Interfaces;

namespace Waketree.Neptune.Interpreter
{
    public sealed class LocalConsole : IConsole
    {
        public void PrintLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
