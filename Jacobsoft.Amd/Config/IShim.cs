using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Config
{
    /// <summary>
    /// Interface representing the configuration of a shim, which adapts a non-AMD JavaScript file to operate as an AMD module
    /// </summary>
    public interface IShim
    {
        string Id { get; }
        string Export { get; }
        IEnumerable<string> Dependencies { get; }
    }
}
