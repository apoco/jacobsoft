using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Exceptions
{
    public class ModuleResolutionException : AmdException
    {
        public ModuleResolutionException(string moduleName, string message) : base(message)
        {
            this.ModuleName = moduleName;
        }

        public string ModuleName { get; private set; }
    }
}
