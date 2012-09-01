﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;
using Jacobsoft.Amd.Internals.Config;

namespace Jacobsoft.Amd.Config
{
    public class AmdConfiguration : IAmdConfiguration
    {
        public static IAmdConfiguration Current 
        {
            get { return ServiceLocator.Instance.Get<IAmdConfiguration>(); }
        }

        internal AmdConfiguration(IAmdConfigurationSection configSection)
        {
            if (configSection != null)
            {
                this.LoaderUrl = configSection.LoaderUrl;
                this.ModuleRootUrl = configSection.RootModuleUrl;
                this.ScriptLoadingMode = configSection.ScriptLoadingMode;
                this.Shims = configSection.Shims.ToDictionary(s => s.Id);
            }

            this.VersionProvider = configSection.VersionProvider == null
                ? ServiceLocator.Instance.Get<IVersionProvider>()
                : Activator.CreateInstance(configSection.VersionProvider) as IVersionProvider;
        }

        public string LoaderUrl { get; set; }

        public string ModuleRootUrl { get; set; }

        public IVersionProvider VersionProvider { get; set; }

        public IScriptMinifier Minifier { get; set; }

        public ScriptLoadingMode ScriptLoadingMode { get; set; }

        public IDictionary<string, IShim> Shims { get; private set; }
    }
}
