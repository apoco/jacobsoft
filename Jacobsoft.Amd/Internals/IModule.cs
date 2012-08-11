using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal interface IModule
    {
        string Id { get; }
        string Content { get; }
        IEnumerable<IModule> Dependencies { get; }
    }
}
