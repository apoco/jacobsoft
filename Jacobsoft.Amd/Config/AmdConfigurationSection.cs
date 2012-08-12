using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;
using Jacobsoft.Amd.Internals.Config;

namespace Jacobsoft.Amd.Config
{
    public class AmdConfigurationSection : ConfigurationSection
    {
        private const string RootModuleUrlAttribute = "moduleRootUrl";
        private const string LoaderUrlAttribute = "loaderUrl";
        private const string ScriptLoadingModeAttribute = "mode";
        private const string ShimsElement = "shims";

        [ConfigurationProperty("xmlns")]
        private string Ignored { get; set; }

        [ConfigurationProperty(RootModuleUrlAttribute, DefaultValue="~/Scripts")]
        public string RootModuleUrl 
        {
            get { return base[RootModuleUrlAttribute] as string; }
        }

        [ConfigurationProperty(LoaderUrlAttribute, IsRequired = true)]
        public string LoaderUrl
        {
            get { return base[LoaderUrlAttribute] as string; }
        }

        [ConfigurationProperty(
            ScriptLoadingModeAttribute, 
            DefaultValue = ScriptLoadingMode.Dynamic)]
        [TypeConverter(typeof(ScriptLoadingModeConfigConverter))]
        public ScriptLoadingMode ScriptLoadingMode 
        {
            get { return (ScriptLoadingMode)base[ScriptLoadingModeAttribute]; } 
        }

        [ConfigurationCollection(typeof(Shim), AddItemName="module")]
        [ConfigurationProperty(ShimsElement)]
        public AmdShimCollectionConfigurationSection Shims
        {
            get 
            {
                return base[ShimsElement] as AmdShimCollectionConfigurationSection;
            }
        }

        private class ScriptLoadingModeConfigConverter : ConfigurationConverterBase
        {
            public override object ConvertFrom(
                ITypeDescriptorContext context, 
                CultureInfo culture, 
                object value)
            {
                return Enum.Parse(typeof(ScriptLoadingMode), value.ToString(), true);
            }
        }
    }
}
