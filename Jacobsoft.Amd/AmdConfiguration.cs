using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd
{
    public class AmdConfiguration : IAmdConfiguration
    {
        public string LoaderFilePath { get; set; }

        public string ModuleFolder { get; set; }
    }
}
