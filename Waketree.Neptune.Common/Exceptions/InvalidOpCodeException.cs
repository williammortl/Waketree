using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waketree.Neptune.Common.Exceptions
{
    public class InvalidOpCodeException : Exception
    {
        public InvalidOpCodeException(string message) : base(message) { }
    }
}
