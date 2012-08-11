using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd
{
    public class AmdConfigurationSection : ConfigurationSection
    {
        private const string RootModuleUrlAttribute = "moduleRootUrl";
        private const string LoaderUrlAttribute = "loaderUrl";
        private const string ScriptLoadingModeAttribute = "mode";

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
