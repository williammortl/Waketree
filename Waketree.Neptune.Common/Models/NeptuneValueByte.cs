using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Interfaces;

namespace Waketree.Neptune.Common.Models
{
    public class NeptuneValueByte : NeptuneValueBase, INeptuneValueNumeric
    {
        public NeptuneValueByte() :
            base(VariableType.Byte)
        {
            this.Value = 0;
        }

        public override byte[] ByteValue
        {
            get
            {
                return new byte[1] { this.Value };
            }
        }

        public byte Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override object Clone()
        {
            return new NeptuneValueByte()
            {
                Value = this.Value
            };
        }

        public double ToDouble()
        {
            return Convert.ToDouble(this.Value);    
        }

        public long ToLong()
        {
            return Convert.ToInt64(this.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is NeptuneValueByte)
            {
                return this.Value == ((NeptuneValueByte)obj).Value;
            }
            else if (obj is NeptuneValueLong)
            {
                return this.Value == ((NeptuneValueLong)obj).Value;
            }
            else if (obj is NeptuneValueDouble)
            {
                return this.Value == ((NeptuneValueDouble)obj).Value;
            }
            return false;
        }

        // add
        public static NeptuneValueDouble operator +(NeptuneValueByte l, NeptuneValueDouble r)
        {
            return new NeptuneValueDouble()
            {
                Value = l.Value + r.Value
            };
        }

        public static NeptuneValueLong operator +(NeptuneValueByte l, NeptuneValueLong r)
        {
            return new NeptuneValueLong()
            {
                Value = l.Value + r.Value
            };
        }

        public static NeptuneValueLong operator +(NeptuneValueByte l, NeptuneValueByte r)
        {
            return new NeptuneValueLong()
            {
                Value = l.Value + r.Value
            };
        }
    }
}
