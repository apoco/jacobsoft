using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd
{
    /// <summary>
    /// Class used for configuring the AMD module functionality.
    /// </summary>
    public static class AmdSystem
    {
        static AmdSystem()
        {
            Configuration = new AmdConfiguration();
        }

        public static AmdConfiguration Configuration { get; private set; }
    }
}
