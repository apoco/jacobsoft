using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Exceptions
{
    public class InvalidNamedModuleException : ModuleResolutionException
    {
        public InvalidNamedModuleException(string moduleName, string message) 
            : base(moduleName, message)
        {
        }
    }
}
