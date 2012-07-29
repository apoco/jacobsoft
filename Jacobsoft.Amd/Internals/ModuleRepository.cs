using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal class ModuleRepository : IModuleRepository
    {
        private readonly IDictionary<string, IModule> modules = new ConcurrentDictionary<string, IModule>();

        public void Add(IModule module)
        {
            this.modules[module.Name] = module;
        }

        public bool TryGetModule(string moduleName, out IModule module)
        {
            return this.modules.TryGetValue(moduleName, out module);
        }
    }
}
