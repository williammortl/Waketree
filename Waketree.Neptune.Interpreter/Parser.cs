using Waketree.Neptune.Common.Models;
using Waketree.Neptune.Common;

namespace Waketree.Neptune.Interpreter
{
    public static class Parser
    {
        public static List<Operation> ParseBytecode(string filename)
        {
            return Parser.ParseBytecode(File.ReadAllBytes(filename));
        }

        public static List<Operation> ParseBytecode(byte[] bytecode)
        {
            var location = 0;
            var retVal = new List<Operation>();
            while (location < bytecode.Length)
            {
                var operation = new Operation();
                operation.OpCode = OpCodeUtils.ByteToOpcode(bytecode[location]);
                location++;
                var args = OpCodeUtils.OpcodeNumParameters(operation.OpCode);
                for (var i = 0; i < args.Count; i++)
                {
                    switch (args[i])
                    {
                        case VariableType.Byte:
                            {
                                operation.Parameters.Add(
                                    new NeptuneValueByte()
                                    {
                                        Value = bytecode[location]
                                    });
                                location += 1;
                                break;
                            }
                        case VariableType.Long:
                            {
                                operation.Parameters.Add(
                                    new NeptuneValueLong()
                                    {
                                        Value = Utils.BytesToLong(bytecode, location)
                                    });
                                location += 8;
                                break;
                            }
                        case VariableType.Double:
                            {
                                operation.Parameters.Add(
                                    new NeptuneValueDouble()
                                    {
                                        Value = Utils.BytesToDouble(bytecode, location)
                                    });
                                location += 8;
                                break;
                            }
                        case VariableType.String:
                            {
                                var newParameter = new NeptuneValueString()
                                {
                                    Value = Utils.NeptuneStringBytesToString(bytecode, location)
                                };
                                operation.Parameters.Add(newParameter);
                                location += 8 + newParameter.Value.Length;
                                break;
                            }
                    }
                }
                retVal.Add(operation);
            }
            return retVal;
        }
    }
}
