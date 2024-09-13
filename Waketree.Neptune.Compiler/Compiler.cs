using Waketree.Neptune.Common;
using Waketree.Neptune.Common.Exceptions;

namespace Waketree.Neptune.Compiler
{
    public sealed class Compiler
    {
        private const char OPCODE_ARG_SPLIT = ',';

        private string code;

        public Compiler(string code)
        {
            this.code = code;
        }

        public byte[] GenerateBytecode()
        {
            var bytecode = new List<byte>();
            var linesArray = this.code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            // clean up lines
            for (int i = 0; i < linesArray.Length; i++)
            {
                linesArray[i] = linesArray[i].Trim();
            }
            var linesList = this.RemoveComments(linesArray);

            // emit bytecode!
            foreach(var line in linesList)
            {
                var lineSplit = line.Split(Compiler.OPCODE_ARG_SPLIT);
                var opCode = OpCodeUtils.StringToOpcode(lineSplit[0]);
                if (opCode != OpCode.INVALID)
                {
                    var argumentsTypes = OpCodeUtils.OpcodeNumParameters(opCode);
                    if (argumentsTypes.Count == lineSplit.Length - 1)
                    {
                        bytecode.Add((byte)opCode);
                        if (argumentsTypes.Count > 0)
                        {
                            for (int i = 1; i <= argumentsTypes.Count; i++)
                            {
                                byte[]? argumentBytes = null;
                                switch (argumentsTypes[i - 1])
                                {
                                    case VariableType.Long:
                                        {
                                            try
                                            {
                                                argumentBytes = Utils.LongToBytes(Convert.ToInt64(lineSplit[i]));
                                            }
                                            catch (Exception e)
                                            {
                                                throw new InvalidOpCodeArgumentException(
                                                    string.Format("Expected long but got: {0}",
                                                        lineSplit[i]),
                                                    e);
                                            }
                                            break;
                                        }
                                    case VariableType.Double:
                                        {
                                            try
                                            {
                                                argumentBytes = Utils.DoubleToBytes(Convert.ToDouble(lineSplit[i]));
                                            }
                                            catch (Exception e)
                                            {
                                                throw new InvalidOpCodeArgumentException(
                                                    string.Format("Expected double but got: {0}",
                                                        lineSplit[i]),
                                                    e);
                                            }
                                            break;
                                        }
                                    case VariableType.Byte:
                                        {
                                            try
                                            {
                                                argumentBytes = new byte[1] { Convert.ToByte(lineSplit[i]) };
                                            }
                                            catch (Exception e)
                                            {
                                                throw new InvalidOpCodeArgumentException(
                                                    string.Format("Expected byte but got: {0}",
                                                        lineSplit[i]),
                                                    e);
                                            }
                                            break;
                                        }
                                    case VariableType.String:
                                        {
                                            argumentBytes = Utils.StringToNeptuneStringBytes(lineSplit[i]);
                                            break;
                                        }
                                }
                                if (argumentBytes != null)
                                {
                                    bytecode.AddRange(argumentBytes);
                                }
                                else
                                {
                                    throw new InvalidOpCodeArgumentException(
                                        string.Format("Argument {0} was for op code {1} invalid",
                                            i.ToString(),
                                            Enum.GetName(opCode)));
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new WrongNumberOfArgumentsException(
                            string.Format("{0} expects {1} arguments, {2} were provided",
                                Enum.GetName(opCode),
                                argumentsTypes.Count.ToString(),
                                lineSplit.Length - 1));
                    }
                }
                else
                {
                    throw new InvalidOpCodeException(string.Format("{0} was an invalid op code for Neptune",
                        lineSplit[0]));
                }
            }

            return bytecode.ToArray();
        }

        private List<string> RemoveComments(string[] lines)
        {
            var fixedCode = new List<string>();
            foreach (var line in lines)
            {
                var loc = line.IndexOf("//");
                if (loc < 0)
                {
                    fixedCode.Add(line);
                }
                else if (loc == 0)
                {
                    // do nothing and skip this line
                }
                else
                {
                    var cleaned = line.Substring(0, loc).Trim();
                    fixedCode.Add(cleaned);
                }
            }
            return fixedCode;
        }
    }
}
