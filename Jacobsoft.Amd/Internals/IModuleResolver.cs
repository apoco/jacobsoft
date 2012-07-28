using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal interface IModuleResolver
    {
        IModule Resolve(string moduleName);
    }
}
