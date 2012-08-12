﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd.Config
{
    public class AmdConfiguration : IAmdConfiguration
    {
        public static IAmdConfiguration Current 
        {
            get { return ServiceLocator.Instance.Get<IAmdConfiguration>(); }
        }

        internal AmdConfiguration()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            if (configSection != null)
            {
                this.LoaderUrl = configSection.LoaderUrl;
                this.ModuleRootUrl = configSection.RootModuleUrl;
                this.ScriptLoadingMode = configSection.ScriptLoadingMode;
                this.Shims = configSection.Shims.ToDictionary(s => s.Id);
            }
        }

        public string LoaderUrl { get; set; }

        public string ModuleRootUrl { get; set; }

        public ScriptLoadingMode ScriptLoadingMode { get; set; }

        public IDictionary<string, IShim> Shims { get; private set; }
    }
}