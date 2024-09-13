using System.Text;
using Waketree.Neptune.Common.Bases;

namespace Waketree.Neptune.Common.Models
{
    public sealed class NeptuneValueString : NeptuneValueBase
    {
        private string value;
        private byte[]? valueBytes;

        public NeptuneValueString() :
            base(VariableType.String)
        { 
            this.value = string.Empty;
            this.valueBytes = null;
        }

        public override byte[] ByteValue
        {
            get
            {
                if (this.valueBytes == null)
                {
                    this.valueBytes = Encoding.UTF8.GetBytes(this.Value);
                }
                return this.valueBytes;
            }
        }

        public override Type NeptuneType
        {
            get
            {
                return this.GetType();
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.valueBytes = null;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is NeptuneValueString)
            {
                return this.Value == ((NeptuneValueString)obj).Value;
            }
            return false;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public override object Clone()
        {
            return new NeptuneValueString()
            {
                Value = (string)this.Value.Clone()
            };
        }
    }
}
