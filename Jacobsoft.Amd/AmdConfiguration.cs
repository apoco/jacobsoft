using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd
{
    public class AmdConfiguration : IAmdConfiguration
    {
        private static ObjectFactory objectFactory = new ObjectFactory();
        private static IAmdConfiguration config = null;

        public static IAmdConfiguration Current 
        {
            get
            {
                return config ?? (config = objectFactory.GetConfiguration());
            }
            set { config = value; } 
        }

        public string LoaderFilePath { get; set; }

        public string ModuleFolder { get; set; }
    }
}
