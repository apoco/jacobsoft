using System;
using System.Collections.Generic;
using System.Configuration;
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

        internal AmdConfiguration()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            if (configSection != null)
            {
                this.ModuleRootUrl = configSection.RootModuleUrl;
                this.IsOptimizationEnabled = configSection.IsOptimizationEnabled;
            }
        }

        public string LoaderFilePath { get; set; }

        public string ModuleRootUrl { get; set; }

        public bool IsOptimizationEnabled { get; set; }
    }
}
