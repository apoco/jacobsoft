using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Exceptions
{
    public class AmdException : Exception
    {
        public AmdException(string message) : base(message)
        {
        }
    }
}
