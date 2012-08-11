using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Exceptions
{
    public class AmdConfigurationException : AmdException
    {
        public AmdConfigurationException(string message)
            : base(message)
        {
        }
    }
}
