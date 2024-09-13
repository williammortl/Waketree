using Waketree.Neptune.Interpreter;

if (args.Length == 0)
{
    Console.WriteLine("\r\nUsage:\r\n\tWaketree.Neptune.Runtime.Local.exe {input filename} {OPTIONAL: 1...n arguments for program}\r\n");
    return;
}

var bytecode = Parser.ParseBytecode(args[0]);
var neptuneProgram = new NeptuneProgram(
    new LocalConsole(),
    bytecode);
var frame = new LocalFrame(bytecode);
neptuneProgram.Execute(args, frame);

// wait for all threads to exit
while (neptuneProgram.RunningThreads.Count > 0)
{
    Thread.Sleep(500);
}
