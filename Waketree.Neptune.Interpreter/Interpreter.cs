using Waketree.Neptune.Common;
using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Exceptions;
using Waketree.Neptune.Common.Interfaces;
using Waketree.Neptune.Common.Models;

namespace Waketree.Neptune.Interpreter
{
    public sealed class Interpreter
    {
        private IConsole console;
        private Operation[] bytecode;
        private Action<string, IConsole, IFrame, int> threadCallback;
        private Random random;

        public string ProcessID { get; private set; }

        public Interpreter(
            string processID,
            IConsole console,
            List<Operation> bytecode,
            Action<string, IConsole, IFrame, int> threadCallback)
        {
            this.console = console;
            this.bytecode = bytecode.ToArray();
            this.threadCallback = threadCallback;
            this.random = new Random();
            this.ProcessID = processID;
        }

        public void Interpret(IFrame frame, int address, CancellationToken cancelToken)
        {
            // main interpreter loop!
            while ((!cancelToken.IsCancellationRequested) && 
                (address < this.bytecode.Length))
            {
                var current = this.bytecode[address];
                switch(current.OpCode)
                {
                    case OpCode.NOP:
                        {
                            Thread.Sleep(0);

                            address++;
                            break;
                        }
                    case OpCode.INVALID:
                    case OpCode.LBL:
                        {
                            // do nothing

                            address++;
                            break;
                        }
                    case OpCode.PUSHB:
                    case OpCode.PUSHL:
                    case OpCode.PUSHD:
                    case OpCode.PUSHSTR:
                        {
                            frame.Push(current.Parameters[0]);
                            
                            address++;
                            break;
                        }
                    case OpCode.POP:
                        {
                            frame.Pop();
                            
                            address++;
                            break;
                        }
                    case OpCode.FLIP:
                        {
                            var pop1 = frame.Pop();
                            var pop2 = frame.Pop();
                            frame.Push(pop1);
                            frame.Push(pop2);

                            address++;
                            break;
                        }
                    case OpCode.DUP:
                        {
                            var pop1 = frame.Pop();
                            frame.Push((NeptuneValueBase)pop1.Clone());
                            frame.Push((NeptuneValueBase)pop1.Clone());

                            address++;
                            break;
                        }
                    case OpCode.TOSTR:
                        {
                            var val1 = frame.Pop();
                            frame.Push(new NeptuneValueString()
                            {
                                Value = val1.ToString()
                            });

                            address++;
                            break;
                        }
                    case OpCode.STRTOL:
                        {
                            var val1 = frame.Pop();
                            if (val1.Type != VariableType.String)
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} requires a string argument", 
                                    this.ProcessID,
                                    current.OpCode.ToString()));
                            }
                            var str1 = (NeptuneValueString)val1;
                            try
                            {
                                frame.Push(new NeptuneValueLong()
                                {
                                    Value = Convert.ToInt64(str1.Value)
                                });
                            }
                            catch (FormatException fe)
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} could not convert \"{2}\" to long",
                                        this.ProcessID,    
                                        current.OpCode,
                                        str1.Value),
                                    fe);
                            }

                            address++;
                            break;
                        }
                    case OpCode.STRTOD:
                        {
                            var val1 = frame.Pop();
                            if (val1.Type != VariableType.String)
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} requires a string arguemnt", 
                                        this.ProcessID, 
                                        current.OpCode.ToString()));
                            }
                            var str1 = (NeptuneValueString)val1;
                            try
                            {
                                frame.Push(new NeptuneValueDouble()
                                {
                                    Value = Convert.ToDouble(str1.Value)
                                });
                            }
                            catch (FormatException fe)
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} could not convert \"{2}\" to double",
                                        this.ProcessID, 
                                        current.OpCode,
                                        str1.Value),
                                    fe);
                            }

                            address++;
                            break;
                        }
                    case OpCode.PRNT:
                        {
                            var val1 = frame.Pop();
                            console.PrintLine(val1.ToString());

                            address++;
                            break;
                        }
                    case OpCode.ADD:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    return v2 + v1;
                                });

                            address++;
                            break;
                        }
                    case OpCode.SUB:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    return v2 - v1;
                                });

                            address++;
                            break;
                        }
                    case OpCode.MUL:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    return v2 * v1;
                                });

                            address++;
                            break;
                        }
                    case OpCode.DIV:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    return v2 / v1;
                                });

                            address++;
                            break;
                        }
                    case OpCode.MOD:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    return v2 % v1;
                                });

                            address++;
                            break;
                        }
                    case OpCode.PWR:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    var v1Num = (INeptuneValueNumeric)v1;
                                    var v2Num = (INeptuneValueNumeric)v2;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Pow(v2Num.ToDouble(), v1Num.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.ROT:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    var v1Num = (INeptuneValueNumeric)v1;
                                    var v2Num = (INeptuneValueNumeric)v2;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Pow(v2Num.ToDouble(), 1 / v1Num.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.LOG:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    var v1Num = (INeptuneValueNumeric)v1;
                                    var v2Num = (INeptuneValueNumeric)v2;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Log(v2Num.ToDouble(), v1Num.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.RAND:
                        {
                            frame.Push(new NeptuneValueDouble()
                            {
                                Value = this.random.NextDouble()
                            });

                            address++;
                            break;
                        }
                    case OpCode.RND:
                        {
                            var val1 = frame.Pop();
                            this.EvaluateUnaryOp(current.OpCode, frame, val1,
                                (v) =>
                                {
                                    var vNum = (INeptuneValueNumeric)v;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = vNum.ToLong()
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.LSHFT:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    return val2 << val1;
                                });

                            address++;
                            break;
                        }
                    case OpCode.RSHFT:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            this.EvaluateBinOp(current.OpCode, frame, val1, val2,
                                (v1, v2) =>
                                {
                                    return val2 >> val1;
                                });

                            address++;
                            break;
                        }
                    case OpCode.SIN:
                        {
                            var val1 = frame.Pop();
                            this.EvaluateUnaryOp(current.OpCode, frame, val1,
                                (v) =>
                                {
                                    var vNum = (INeptuneValueNumeric)v;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Sin(vNum.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.COS:
                        {
                            var val1 = frame.Pop();
                            this.EvaluateUnaryOp(current.OpCode, frame, val1,
                                (v) =>
                                {
                                    var vNum = (INeptuneValueNumeric)v;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Cos(vNum.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.TAN:
                        {
                            var val1 = frame.Pop();
                            this.EvaluateUnaryOp(current.OpCode, frame, val1,
                                (v) =>
                                {
                                    var vNum = (INeptuneValueNumeric)v;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Tan(vNum.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.ASIN:
                        {
                            var val1 = frame.Pop();
                            this.EvaluateUnaryOp(current.OpCode, frame, val1,
                                (v) =>
                                {
                                    var vNum = (INeptuneValueNumeric)v;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Asin(vNum.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.ACOS:
                        {
                            var val1 = frame.Pop();
                            this.EvaluateUnaryOp(current.OpCode, frame, val1,
                                (v) =>
                                {
                                    var vNum = (INeptuneValueNumeric)v;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Acos(vNum.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.ATAN:
                        {
                            var val1 = frame.Pop();
                            this.EvaluateUnaryOp(current.OpCode, frame, val1,
                                (v) =>
                                {
                                    var vNum = (INeptuneValueNumeric)v;
                                    return new NeptuneValueDouble()
                                    {
                                        Value = Math.Sin(vNum.ToDouble())
                                    };
                                });

                            address++;
                            break;
                        }
                    case OpCode.GOTO:
                        {
                            
                            var labelGoto = current.Parameters[0].ToString();
                            var addressGoto = frame.LookupLabel(labelGoto);
                            if (addressGoto == -1)
                            {
                                throw new LabelNotFoundException(
                                    string.Format("Process: {0}, goto label {1} was not found", 
                                        this.ProcessID, 
                                        labelGoto));
                            }

                            address = addressGoto;
                            break;
                        }
                    case OpCode.GOSUB:
                        {
                            var labelGosub = current.Parameters[0].ToString();
                            var addressGosub = frame.LookupLabel(labelGosub);
                            if (addressGosub == -1)
                            {
                                throw new LabelNotFoundException(
                                    string.Format("Process: {0}, gosub label {1} was not found", 
                                        this.ProcessID, 
                                        labelGosub));
                            }

                            // blocks until this Gosub completes
                            this.Interpret(frame.GetChildFrame(), addressGosub, cancelToken);

                            address++;
                            break;
                        }
                    case OpCode.TGOSUB:
                        {
                            var labelTsub = current.Parameters[0].ToString();
                            var addressTsub = frame.LookupLabel(labelTsub);
                            if (addressTsub == -1)
                            {
                                throw new LabelNotFoundException(
                                    string.Format("Process: {0}, thread gosub label {1} was not found", 
                                        this.ProcessID, 
                                        labelTsub));
                            }

                            // callback to thread handler and launch new thread / interpreter
                            this.threadCallback(this.ProcessID,
                                this.console,
                                frame.GetChildFrame(), 
                                addressTsub);

                            address++;
                            break;
                        }
                    case OpCode.IFQ:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();

                            address = this.EvaluateIf(current.OpCode, val1, val2, address,
                                current.Parameters[0].ToString(), frame,
                                (v1, v2) =>
                                {
                                    return v2 == v1;
                                });

                            break;
                        }
                    case OpCode.IFNQ:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();

                            address = this.EvaluateIf(current.OpCode, val1, val2, address,
                                current.Parameters[0].ToString(), frame,
                                (v1, v2) =>
                                {
                                    return v2 != v1;
                                });

                            break;
                        }
                    case OpCode.IFG:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();

                            address = this.EvaluateIf(current.OpCode, val1, val2, address,
                                current.Parameters[0].ToString(), frame,
                                (v1, v2) =>
                                {
                                    return v2 > v1;
                                });

                            break;
                        }
                    case OpCode.IFL:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();

                            address = this.EvaluateIf(current.OpCode, val1, val2, address,
                                current.Parameters[0].ToString(), frame,
                                (v1, v2) =>
                                {
                                    return v2 < v1;
                                });

                            break;
                        }
                    case OpCode.IFGQ:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();

                            address = this.EvaluateIf(current.OpCode, val1, val2, address,
                                current.Parameters[0].ToString(), frame,
                                (v1, v2) =>
                                {
                                    return v2 >= v1;
                                });

                            break;
                        }
                    case OpCode.IFLQ:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();

                            address = this.EvaluateIf(current.OpCode, val1, val2, address,
                                current.Parameters[0].ToString(), frame,
                                (v1, v2) =>
                                {
                                    return v2 <= v1;
                                });

                            break;
                        }
                    case OpCode.LOCAL_SAV:
                        {
                            var value = frame.Pop();
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            frame.StoreLocal(name, value);

                            address++;
                            break;
                        }
                    case OpCode.LOCAL_LOD:
                        {
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            var value = frame.GetLocal(name);
                            if (value == null)
                            {
                                throw new VariableNotFoundException(
                                    string.Format("Process: {0}, could not find local variable {1} from op code {2}",
                                    this.ProcessID, 
                                    name.ToString(),
                                    current.OpCode));
                            }
                            frame.Push(value);

                            address++;
                            break;
                        }
                    case OpCode.LOCAL_LOD_BYTE:
                        {
                            var val1 = frame.Pop();
                            if (val1.Type != VariableType.Byte)
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} requires a byte argument", 
                                        this.ProcessID,
                                        current.OpCode.ToString()));

                            }
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            var varValue = frame.GetLocal(name);
                            if (varValue == null)
                            {
                                throw new VariableNotFoundException(
                                    string.Format("Process: {0}, could not find local variable {1} from op code {2}",
                                        this.ProcessID, 
                                        name.ToString(),
                                        current.OpCode));
                            }
                            frame.Push(new NeptuneValueByte()
                            {
                                Value = varValue.ByteValue[((NeptuneValueByte)val1).Value]
                            });

                            address++;
                            break;
                        }
                    case OpCode.LOCAL_SAV_BYTE:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            if ((val1.Type != VariableType.Long) ||
                                (val2.Type != VariableType.Byte))
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} requires a long and a byte argument",
                                        this.ProcessID, 
                                        current.OpCode.ToString()));

                            }
                            var val1Long = (NeptuneValueLong)val1;
                            var val2Byte = (NeptuneValueByte)val2;
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            var varValue = frame.GetLocal(name);
                            if (varValue == null)
                            {
                                throw new VariableNotFoundException(
                                    string.Format("Process: {0}, could not find local variable {1} from op code {2}",
                                        this.ProcessID, 
                                        name.ToString(),
                                        current.OpCode));
                            }
                            varValue.ByteValue[(int)val1Long.Value] = val2Byte.Value;

                            address++;
                            break;
                        }
                    case OpCode.LOCAL_DELETE:
                        {
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            frame.StoreLocal(name, null);

                            address++;
                            break;
                        }
                    case OpCode.GLOBAL_SAV:
                        {
                            var value = frame.Pop();
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            frame.StoreGlobal(name, value);

                            address++;
                            break;
                        }
                    case OpCode.GLOBAL_LOD:
                        {
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            var value = frame.GetGlobal(name);
                            if (value == null)
                            {
                                throw new VariableNotFoundException(
                                    string.Format("Process: {0}, could not find global variable {1} from op code {2}",
                                        this.ProcessID, 
                                        name.ToString(),
                                        current.OpCode));
                            }
                            frame.Push(value);

                            address++;
                            break;
                        }
                    case OpCode.GLOBAL_LOD_BYTE:
                        {
                            var val1 = frame.Pop();
                            if (val1.Type != VariableType.Byte)
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} requires a byte argument",
                                        this.ProcessID, 
                                        current.OpCode.ToString()));

                            }
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            var varValue = frame.GetGlobal(name);
                            if (varValue == null)
                            {
                                throw new VariableNotFoundException(
                                    string.Format("Process: {0}, could not find global variable {1} from op code {2}",
                                        this.ProcessID, 
                                        name.ToString(),
                                        current.OpCode));
                            }
                            frame.Push(new NeptuneValueByte()
                            {
                                Value = varValue.ByteValue[((NeptuneValueByte)val1).Value]
                            });

                            address++;
                            break;
                        }
                    case OpCode.GLOBAL_SAV_BYTE:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            if ((val1.Type != VariableType.Long) ||
                                (val2.Type != VariableType.Byte))
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} requires a long and a byte argument",
                                        this.ProcessID, 
                                        current.OpCode.ToString()));

                            }
                            var val1Long = (NeptuneValueLong)val1;
                            var val2Byte = (NeptuneValueByte)val2;
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            var varValue = frame.GetGlobal(name);
                            if (varValue == null)
                            {
                                throw new VariableNotFoundException(
                                    string.Format("Process: {0}, could not find global variable {1} from op code {2}",
                                        this.ProcessID, 
                                        name.ToString(),
                                        current.OpCode));
                            }
                            varValue.ByteValue[(int)val1Long.Value] = val2Byte.Value;

                            address++;
                            break;
                        }
                    case OpCode.GLOBAL_DELETE:
                        {
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            frame.StoreGlobal(name, null);

                            address++;
                            break;
                        }
                    case OpCode.CRIT_ENTER:
                        {
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            while (!frame.EnterCriticalSection(name))
                            {
                                Thread.Sleep(0);
                            }

                            address++;
                            break;
                        }
                    case OpCode.CRIT_EXIT:
                        {
                            var name = ((INeptuneValueNumeric)current.Parameters[0]).ToLong();
                            frame.ExitCriticalSection(name);

                            address++;
                            break;
                        }
                    case OpCode.AND:
                    case OpCode.OR:
                    case OpCode.XOR:
                        {
                            var val1 = frame.Pop();
                            var val2 = frame.Pop();
                            if ((val1.Type != VariableType.Byte) ||
                                (val2.Type != VariableType.Byte))
                            {
                                throw new InvalidOpCodeArgumentException(
                                    string.Format("Process: {0}, op code {1} requires 2 bytes", 
                                        this.ProcessID, 
                                        current.OpCode.ToString()));
                            }
                            var byte1 = (NeptuneValueByte)val1;
                            var byte2 = (NeptuneValueByte)val2;

                            byte result = 0;
                            if (current.OpCode == OpCode.AND)
                            {
                                result = (byte)(byte2.Value & byte1.Value);
                            }
                            else if (current.OpCode == OpCode.OR)
                            {
                                result = (byte)(byte2.Value | byte1.Value);
                            }
                            else
                            {
                                result = (byte)(byte2.Value ^ byte1.Value);
                            }
                            frame.Push(new NeptuneValueByte
                            {
                                Value = result
                            });

                            address++;
                            break;
                        }
                    case OpCode.END:
                        {
                            return;
                        }
                }
            }
        }

        private int EvaluateIf(
            OpCode opcode,
            NeptuneValueBase val1, 
            NeptuneValueBase val2, 
            int address,
            string labelName,
            IFrame frame,
            Func<NeptuneValueBase, NeptuneValueBase, bool> comparator)
        {
            var nextAddress = address + 1;
            try
            {
                var ifSwitch = comparator(val2, val1);
                if (!ifSwitch)
                {
                    // if condition was false so jump to end of if block label
                    var addressJump = frame.LookupLabel(labelName);
                    if (addressJump == -1)
                    {
                        throw new LabelNotFoundException(
                            string.Format("Process: {0}, op code {1} could not find label: {2}",
                                this.ProcessID,
                                opcode,
                                labelName));
                    }
                    nextAddress = addressJump;
                }
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(
                    string.Format("Process: {0}, invalid comparison for op code {1}",
                    this.ProcessID, opcode), e);
            }
            return nextAddress;
        }

        private void EvaluateBinOp(
            OpCode opcode,
            IFrame frame,
            NeptuneValueBase val1,
            NeptuneValueBase val2,
            Func<NeptuneValueBase, NeptuneValueBase, NeptuneValueBase> binOp)
        {
            try
            {
                frame.Push(binOp(val1, val2));
            }
            catch
            {
                throw new InvalidOpCodeArgumentException(
                    string.Format("Process: {0}, op code {1} failed with arguments: {2}, {3}",
                    this.ProcessID,
                    nameof(opcode),
                    val1.ToString(),
                    val2.ToString()));
            }
        }

        private void EvaluateUnaryOp(
            OpCode opcode,
            IFrame frame,
            NeptuneValueBase val1,
            Func<NeptuneValueBase, NeptuneValueBase> unaryOp)
        {
            try
            {
                frame.Push(unaryOp(val1));
            }
            catch
            {
                throw new InvalidOpCodeArgumentException(
                    string.Format("Process: {0}, op code {1} failed with argument: {2}",
                    this.ProcessID,
                    nameof(opcode),
                    val1.ToString()));
            }
        }
    }
}
