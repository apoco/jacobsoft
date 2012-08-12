using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Config
{
    public interface IAmdConfiguration
    {
        string LoaderUrl { get; set; }

        string ModuleRootUrl { get; set; }

        ScriptLoadingMode ScriptLoadingMode { get; set; }

        IVersionProvider VersionProvider { get; set; }

        IDictionary<string, IShim> Shims { get; }
    }
}
