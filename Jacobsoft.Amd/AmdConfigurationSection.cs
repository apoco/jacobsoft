using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd
{
    public class AmdConfigurationSection : ConfigurationSection
    {
        private const string RootModuleUrlAttribute = "moduleRootUrl";
        private const string IsOptimizationEnabledAttribute = "optimize";
        private const string LoaderUrlAttribute = "loaderUrl";

        [ConfigurationProperty("xmlns")]
        private string Ignored { get; set; }

        [ConfigurationProperty(RootModuleUrlAttribute, DefaultValue="~/Scripts")]
        public string RootModuleUrl 
        {
            get { return base[RootModuleUrlAttribute] as string; }
            set { base[RootModuleUrlAttribute] = value; }
        }

        [ConfigurationProperty(LoaderUrlAttribute, IsRequired = true)]
        public string LoaderUrl
        {
            get { return base[LoaderUrlAttribute] as string; }
            set { base[LoaderUrlAttribute] = value; }
        }

        [ConfigurationProperty(IsOptimizationEnabledAttribute, DefaultValue = false)]
        public bool IsOptimizationEnabled 
        {
            get { return (bool)base[IsOptimizationEnabledAttribute]; }
            set { base[IsOptimizationEnabledAttribute] = value; }
        }
    }
}
