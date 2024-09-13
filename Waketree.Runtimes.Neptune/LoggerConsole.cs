using Microsoft.Extensions.Logging;
using Waketree.Common.Interfaces;
using Waketree.Common.Models;
using Waketree.Neptune.Common.Interfaces;

namespace Waketree.Neptune.Interpreter
{
    public sealed class LoggerConsole : IConsole
    {
        private const string LoggerMessage = "Process ID: {0}, Thread ID: {1}\r\nMessage:\r\n{2}";

        private ILogger<IRuntime> logger;
        private WaketreeThread thread;

        public LoggerConsole(ILogger<IRuntime> logger, WaketreeThread thread)
        {
            this.logger = logger;
            this.thread = thread;
        }

        public void PrintLine(string message)
        {
            this.logger.LogInformation(string.Format(LoggerConsole.LoggerMessage,
                this.thread.ProcessID, this.thread.ThreadID, message));
        }
    }
}
