using Waketree.Neptune.Common.Interfaces;
using Waketree.Neptune.Common.Models;

namespace Waketree.Neptune.Common.Bases
{
    public abstract class NeptuneValueBase : ICloneable
    {
        public VariableType Type { get; }

        public virtual bool IsNumeric
        {
            get
            {
                return !(this.Type == VariableType.String);
            }
        }

        public virtual Type NeptuneType { get; }
        public virtual byte[] ByteValue { get; }

        public NeptuneValueBase(VariableType type)
        {
            this.Type = type;
        }

        public abstract override string ToString();

        public abstract object Clone();

        public static NeptuneValueBase? ObjectToValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is NeptuneValueString)
            {
                return (NeptuneValueString)value;
            }
            else if (value is NeptuneValueLong)
            {
                return (NeptuneValueLong)value;
            }
            else if (value is NeptuneValueDouble)
            {
                return (NeptuneValueDouble)value;
            }
            else if (value is NeptuneValueByte)
            {
                return (NeptuneValueByte)value;
            }
            else
            {
                return null;
            }
        }

        public static NeptuneValueBase operator +(NeptuneValueBase l, NeptuneValueBase r)
        {
            if ((l.IsNumeric) && (r.IsNumeric))
            {
                var lNum = (INeptuneValueNumeric)l;
                var rNum = (INeptuneValueNumeric)r;
                if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double)) 
                {
                    return new NeptuneValueDouble()
                    {
                        Value = lNum.ToDouble() + rNum.ToDouble()
                    };
                }
                else
                {
                    return new NeptuneValueDouble()
                    {
                        Value = lNum.ToLong() + rNum.ToLong()
                    };
                }
            }
            else
            {
                return new NeptuneValueString()
                {
                    Value = l.ToString() + r.ToString()
                };
            }
        }

        public static NeptuneValueBase operator -(NeptuneValueBase l, NeptuneValueBase r)
        {
            if ((!l.IsNumeric) || (!r.IsNumeric))
            {
                throw new InvalidOperationException("Cannot subtract with strings");
            }
            var lNum = (INeptuneValueNumeric)l;
            var rNum = (INeptuneValueNumeric)r;
            if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double))
            {
                return new NeptuneValueDouble()
                {
                    Value = lNum.ToDouble() - rNum.ToDouble()
                };
            }
            else
            {
                return new NeptuneValueLong()
                {
                    Value = lNum.ToLong() - rNum.ToLong()
                };
            }
        }

        public static NeptuneValueBase operator *(NeptuneValueBase l, NeptuneValueBase r)
        {
            if ((!l.IsNumeric) || (!r.IsNumeric))
            {
                throw new InvalidOperationException("Cannot multiply with strings");
            }
            var lNum = (INeptuneValueNumeric)l;
            var rNum = (INeptuneValueNumeric)r;
            if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double))
            {
                return new NeptuneValueDouble()
                {
                    Value = lNum.ToDouble() * rNum.ToDouble()
                };
            }
            else
            {
                return new NeptuneValueLong()
                {
                    Value = lNum.ToLong() * rNum.ToLong()
                };
            }
        }

        public static NeptuneValueBase operator /(NeptuneValueBase l, NeptuneValueBase r)
        {
            if ((!l.IsNumeric) || (!r.IsNumeric))
            {
                throw new InvalidOperationException("Cannot divide with strings");
            }
            var lNum = (INeptuneValueNumeric)l;
            var rNum = (INeptuneValueNumeric)r;
            return new NeptuneValueDouble()
            {
                Value = lNum.ToDouble() / rNum.ToDouble()
            };
        }

        public static NeptuneValueBase operator %(NeptuneValueBase l, NeptuneValueBase r)
        {
            if ((!l.IsNumeric) || (!r.IsNumeric))
            {
                throw new InvalidOperationException("Cannot modulo with strings");
            }
            else if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double))
            {
                throw new InvalidOperationException("Cannot modulo with non whole numbers");
            }
            var lNum = (INeptuneValueNumeric)l;
            var rNum = (INeptuneValueNumeric)r;
            return new NeptuneValueLong()
            {
                Value = lNum.ToLong() % rNum.ToLong()
            };
        }

        public static NeptuneValueBase operator <<(NeptuneValueBase l, NeptuneValueBase r)
        {
            if ((!l.IsNumeric) || (!r.IsNumeric))
            {
                throw new InvalidOperationException("Cannot left shift with strings");
            }
            else if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double) ||
                (r.Type != VariableType.Byte))
            {
                throw new InvalidOperationException("Can only left shift a long or byte by byte number of bytes");
            }
            var lNum = (INeptuneValueNumeric)l;
            var rNum = (NeptuneValueByte)r;
            return new NeptuneValueLong()
            {
                Value = lNum.ToLong() << rNum.Value
            };
        }

        public static NeptuneValueBase operator >>(NeptuneValueBase l, NeptuneValueBase r)
        {
            if ((!l.IsNumeric) || (!r.IsNumeric))
            {
                throw new InvalidOperationException("Cannot right shift with strings");
            }
            else if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double) ||
                (r.Type != VariableType.Byte))
            {
                throw new InvalidOperationException("Can only right shift a long or byte by byte number of bytes");
            }
            var lNum = (INeptuneValueNumeric)l;
            var rNum = (NeptuneValueByte)r;
            return new NeptuneValueLong()
            {
                Value = lNum.ToLong() >> rNum.Value
            };
        }

        public static bool operator ==(NeptuneValueBase? l, NeptuneValueBase? r)
        {
            if (object.ReferenceEquals(l, null) || object.ReferenceEquals(r, null))
            {
                return (object.ReferenceEquals(l, null) && object.ReferenceEquals(r, null));
            }
            if (l.IsNumeric && r.IsNumeric)
            {
                var lNum = (INeptuneValueNumeric)l;
                var rNum = (INeptuneValueNumeric)r;
                if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double))
                {
                    return lNum.ToDouble() == rNum.ToDouble();
                }
                return lNum.ToLong() == rNum.ToLong();
            }
            else if (!l.IsNumeric && !r.IsNumeric)
            {
                return (((NeptuneValueString)l).Value == ((NeptuneValueString)r).Value);
            }
            else
            {
                throw new InvalidOperationException("Can compare numerics to numerics and strings to strings");
            }
        }

        public static bool operator !=(NeptuneValueBase l, NeptuneValueBase r)
        {
            return !(l == r);
        }

        public static bool operator >(NeptuneValueBase l, NeptuneValueBase r)
        {
            if (l.IsNumeric && r.IsNumeric)
            {
                var lNum = (INeptuneValueNumeric)l;
                var rNum = (INeptuneValueNumeric)r;
                if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double))
                {
                    return lNum.ToDouble() > rNum.ToDouble();
                }
                return lNum.ToLong() > rNum.ToLong();
            }
            else
            {
                throw new InvalidOperationException("Can only perform inequality comparisons on numerics");
            }
        }

        public static bool operator <(NeptuneValueBase l, NeptuneValueBase r)
        {
            return !(l > r);
        }

        public static bool operator >=(NeptuneValueBase l, NeptuneValueBase r)
        {
            if (l.IsNumeric && r.IsNumeric)
            {
                var lNum = (INeptuneValueNumeric)l;
                var rNum = (INeptuneValueNumeric)r;
                if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double))
                {
                    return lNum.ToDouble() >= rNum.ToDouble();
                }
                return lNum.ToLong() >= rNum.ToLong();
            }
            else
            {
                throw new InvalidOperationException("Can only perform inequality comparisons on numerics");
            }
        }

        public static bool operator <=(NeptuneValueBase l, NeptuneValueBase r)
        {
            if (l.IsNumeric && r.IsNumeric)
            {
                var lNum = (INeptuneValueNumeric)l;
                var rNum = (INeptuneValueNumeric)r;
                if ((l.Type == VariableType.Double) || (r.Type == VariableType.Double))
                {
                    return lNum.ToDouble() <= rNum.ToDouble();
                }
                return lNum.ToLong() <= rNum.ToLong();
            }
            else
            {
                throw new InvalidOperationException("Can only perform inequality comparisons on numerics");
            }
        }
    }
}
