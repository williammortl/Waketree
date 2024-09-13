using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Interfaces;

namespace Waketree.Neptune.Common.Models
{
    public class NeptuneValueLong : NeptuneValueBase, INeptuneValueNumeric
    {

        public NeptuneValueLong() :
            base(VariableType.Long)
        {
            this.Value = 0;
        }

        public override byte[] ByteValue
        {
            get
            {
                return BitConverter.GetBytes(this.Value);
            }
        }

        public long Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override object Clone()
        {
            return new NeptuneValueLong()
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
            return this.Value;
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
        public static NeptuneValueDouble operator + (NeptuneValueLong l, NeptuneValueDouble r)
        {
            return new NeptuneValueDouble()
            {
                Value = l.Value + r.Value
            };
        }

        public static NeptuneValueLong operator +(NeptuneValueLong l, NeptuneValueLong r)
        {
            return new NeptuneValueLong()
            {
                Value = l.Value + r.Value
            };
        }

        public static NeptuneValueLong operator +(NeptuneValueLong l, NeptuneValueByte r)
        {
            return new NeptuneValueLong()
            {
                Value = l.Value + r.Value
            };
        }
    }
}
