using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Config;

namespace Jacobsoft.Amd.Internals
{
    internal class ScriptOutputContext
    {
        public ScriptOutputContext()
        {
            this.WrittenModules = new HashSet<string>();
        }

        public ScriptLoadingMode ScriptLoadingMode 
        {
            get { return AmdConfiguration.Current.ScriptLoadingMode; } 
        }
        
        public bool IsLoaderScriptWritten { get; set; }
        public bool IsConfigScriptWritten { get; set; }
        public ISet<string> WrittenModules { get; private set; }

        internal IModule ResolveModule(string moduleId)
        {
            return ServiceLocator.Instance.Get<IModuleResolver>().Resolve(moduleId);
        }
    }
}
