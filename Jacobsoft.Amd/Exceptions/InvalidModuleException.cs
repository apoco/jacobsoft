using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Exceptions
{
    public class InvalidModuleException : ModuleResolutionException
    {
        public InvalidModuleException(string moduleName, string message) 
            : base(moduleName, message)
        {
        }
    }
}
