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
            return new AmdConfiguration();
        }

        public virtual IModuleResolver GetModuleResolver()
        {
            return new ModuleResolver(
                AmdConfiguration.Current,
                this.GetRepository(),
                this.GetFileSystem());
        }

        public virtual IModuleRepository GetRepository()
        {
            return new ModuleRepository();
        }

        public virtual IFileSystem GetFileSystem()
        {
            return new FileSystem();
        }
    }
}
