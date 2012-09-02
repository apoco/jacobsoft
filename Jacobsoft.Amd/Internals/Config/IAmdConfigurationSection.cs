using Jacobsoft.Amd.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals.Config
{
    internal interface IAmdConfigurationSection
    {
        string LoaderUrl { get; }
        string RootModuleUrl { get; }
        ScriptLoadingMode ScriptLoadingMode { get; }
        Type VersionProvider { get; }
        Type Minifier { get; }
        IEnumerable<IShim> Shims { get; }
    }
}
