using System.Text;
using Waketree.Neptune.Common.Bases;
using Waketree.Neptune.Common.Interfaces;

namespace Waketree.Neptune.Common.Models
{
    public class NeptuneValueDouble : NeptuneValueBase, INeptuneValueNumeric
    {
        public NeptuneValueDouble() :
            base(VariableType.Double)
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

        public double Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override object Clone()
        {
            return new NeptuneValueDouble()
            {
                Value = this.Value
            };
        }

        public double ToDouble()
        {
            return this.Value;
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
        public static NeptuneValueDouble operator +(NeptuneValueDouble l, NeptuneValueDouble r)
        {
            return new NeptuneValueDouble()
            {
                Value = l.Value + r.Value
            };
        }

        public static NeptuneValueDouble operator +(NeptuneValueDouble l, NeptuneValueLong r)
        {
            return new NeptuneValueDouble()
            {
                Value = l.Value + r.Value
            };
        }

        public static NeptuneValueDouble operator +(NeptuneValueDouble l, NeptuneValueByte r)
        {
            return new NeptuneValueDouble()
            {
                Value = l.Value + r.Value
            };
        }
    }
}
