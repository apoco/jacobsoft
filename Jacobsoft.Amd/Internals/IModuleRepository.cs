using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal interface IModuleRepository
    {
        void Add(IModule module);
        bool TryGetModule(string moduleName, out IModule module);
    }
}
