using Waketree.Neptune.Compiler;

if (args.Length == 0)
{
    Console.WriteLine("\r\nUsage:\r\n\tWaketree.Neptune.Compiler {input filename} {OPTIONAL: output filename}\r\n");
    return;
}

var compiler = new Compiler(File.ReadAllText(args[0]));
var bytes = compiler.GenerateBytecode();

var outputFile = args[0] + ".compiled";
if (args.Length > 1)
{
    outputFile = args[1];
}
File.WriteAllBytes(outputFile, bytes);
