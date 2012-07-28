using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Exceptions
{
    public class InvalidPathException : ModuleResolutionException
    {
        public InvalidPathException(string moduleName, string message) 
            : base(moduleName, message)
        {
        }
    }
}
