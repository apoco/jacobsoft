using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd.Internals
{
    internal class ObjectFactory
    {
        public virtual IAmdConfiguration GetConfiguration()
        {
            return AmdSystem.Configuration;
        }

        public virtual IModuleResolver GetModuleResolver()
        {
            return new ModuleResolver(this.GetConfiguration(), this.GetFileSystem());
        }

        public virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }
    }
}
