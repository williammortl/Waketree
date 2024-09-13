using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waketree.Neptune.Common.Exceptions
{
    public class WrongNumberOfArgumentsException : Exception
    {
        public WrongNumberOfArgumentsException(string  message) : base(message) { }
    }
}
