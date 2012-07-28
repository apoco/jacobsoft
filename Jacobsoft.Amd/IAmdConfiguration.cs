using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd
{
    public interface IAmdConfiguration
    {
        string LoaderFilePath { get; }

        string ModuleFolder { get; }
    }
}
