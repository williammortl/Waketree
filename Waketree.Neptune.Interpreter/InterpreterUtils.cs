using Waketree.Neptune.Common;
using Waketree.Neptune.Common.Models;

namespace Waketree.Neptune.Interpreter
{
    public static class InterpreterUtils
    {
        public static Dictionary<string, int> BuildLabelLookup(List<Operation> bytecode)
        {
            var retVal = new Dictionary<string, int>();

            for (var address = 0; address < bytecode.Count; address++)
            {
                if (bytecode[address].OpCode == OpCode.LBL)
                {
                    retVal.Add(bytecode[address].Parameters[0].ToString(), address);
                }
                address++;
            }

            return retVal;
        }
    }
}
