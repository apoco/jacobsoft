using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal class Module : IModule
    {
        public string Id { get; internal set; }
        public string Content { get; internal set; }
        public IEnumerable<IModule> Dependencies { get; internal set; }
    }
}
