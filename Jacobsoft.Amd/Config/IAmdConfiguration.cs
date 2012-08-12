using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Config
{
    public interface IAmdConfiguration
    {
        string LoaderUrl { get; }

        string ModuleRootUrl { get; }

        ScriptLoadingMode ScriptLoadingMode { get; }

        IDictionary<string, IShim> Shims { get; }
    }
}
