using Waketree.Neptune.Common.Exceptions;

namespace Waketree.Neptune.Common
{
    public static class OpCodeUtils
    {
        public static OpCode StringToOpcode(string opCodeString)
        {
            if (Enum.TryParse(typeof(OpCode), opCodeString, out var parsedEnum))
            {
                return (OpCode)parsedEnum;
            }
            else
            {
                return OpCode.INVALID;
            }
        }

        public static byte OpcodeToByte(OpCode opCode)
        {
            return (byte)opCode;
        }

        public static OpCode ByteToOpcode(byte opcode)
        {
            var maxValue = Enum.GetValues<OpCode>().Max();
            if ((opcode < 0) || (opcode > (byte)maxValue))
            {
                throw new InvalidOpCodeException(
                    string.Format("{0} is an invalid op code", opcode.ToString()));
            }
            return (OpCode)opcode;
        }

        public static List<VariableType> OpcodeNumParameters(OpCode opCode)
        {
            var retVal = new List<VariableType>();
            switch (opCode)
            {
                case OpCode.PUSHB:
                    {
                        retVal.Add(VariableType.Byte);
                        return retVal;
                    }
                case OpCode.PUSHL:
                case OpCode.LOCAL_SAV:
                case OpCode.GLOBAL_SAV:
                case OpCode.LOCAL_LOD:
                case OpCode.GLOBAL_LOD:
                case OpCode.LOCAL_LOD_BYTE:
                case OpCode.LOCAL_SAV_BYTE:
                case OpCode.GLOBAL_LOD_BYTE:
                case OpCode.GLOBAL_SAV_BYTE:
                case OpCode.LOCAL_DELETE:
                case OpCode.GLOBAL_DELETE:
                case OpCode.CRIT_ENTER:
                case OpCode.CRIT_EXIT:
                    {
                        retVal.Add(VariableType.Long);
                        return retVal;
                    }
                case OpCode.PUSHD:
                    {
                        retVal.Add(VariableType.Double);
                        return retVal;
                    }
                case OpCode.PUSHSTR:
                case OpCode.LBL:
                case OpCode.GOTO:
                case OpCode.GOSUB:
                case OpCode.TGOSUB:
                case OpCode.IFQ:
                case OpCode.IFNQ:
                case OpCode.IFG:
                case OpCode.IFL:
                case OpCode.IFGQ:
                case OpCode.IFLQ:
                    {
                        retVal.Add(VariableType.String);
                        return retVal;
                    }
                default:
                    {
                        return retVal;
                    }
            }
        }
    }
}
