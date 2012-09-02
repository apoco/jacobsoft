using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Config
{
    public interface IBundle
    {
        string Id { get; }
        IEnumerable<string> Modules { get; }
    }
}
