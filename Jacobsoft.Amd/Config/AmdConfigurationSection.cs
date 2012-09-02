using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals;
using Jacobsoft.Amd.Internals.Config;
using System.Reflection;

namespace Jacobsoft.Amd.Config
{
    public class AmdConfigurationSection : ConfigurationSection, IAmdConfigurationSection
    {
        private const string RootModuleUrlAttribute = "moduleRootUrl";
        private const string LoaderUrlAttribute = "loaderUrl";
        private const string ScriptLoadingModeAttribute = "mode";
        private const string VersionProviderAttribute = "versionProvider";
        private const string MinifierAttribute = "minifier";
        private const string ShimsElement = "shims";
        private const string BundlesElement = "bundles";

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

        [ConfigurationProperty(VersionProviderAttribute, DefaultValue = null)]
        [TypeConverter(typeof(TypeProviderConverter<IVersionProvider>))]
        public Type VersionProvider
        {
            get { return base[VersionProviderAttribute] as Type; }
        }

        [ConfigurationProperty(MinifierAttribute, DefaultValue = null)]
        [TypeConverter(typeof(TypeProviderConverter<IScriptMinifier>))]
        public Type Minifier
        {
            get { return base[MinifierAttribute] as Type; }
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

        [ConfigurationCollection(typeof(AmdBundleConfigurationElement), AddItemName = "bundle")]
        [ConfigurationProperty(BundlesElement)]
        public AmdModuleBundleCollectionConfigurationSection Bundles
        {
            get
            {
                return base[BundlesElement] as AmdModuleBundleCollectionConfigurationSection;
            }
        }

        IEnumerable<IShim> IAmdConfigurationSection.Shims
        {
            get { return this.Shims; }
        }

        IEnumerable<IBundle> IAmdConfigurationSection.Bundles
        {
            get { return this.Bundles; }
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

        private class TypeProviderConverter<BaseType> : ConfigurationConverterBase
        {
            public override object ConvertFrom(
                ITypeDescriptorContext context, 
                CultureInfo culture, 
                object value)
            {
                var typeAndAssemblyName = value.ToString();
                var dividerIdx = typeAndAssemblyName.IndexOf(',');
                if (dividerIdx == -1)
                {
                    throw new ConfigurationErrorsException("You must specify the name of the type and assembly.");
                }

                var assemblyName = typeAndAssemblyName.Substring(dividerIdx + 1).Trim();
                var assembly = Assembly.Load(assemblyName);

                var typeName = typeAndAssemblyName.Substring(0, dividerIdx).Trim();
                var type = assembly.GetType(typeName);
                if (type == null)
                {
                    throw new ConfigurationErrorsException("Type not found: " + typeName);
                }

                var baseType = typeof(BaseType);
                if (!baseType.IsAssignableFrom(type))
                {
                    throw new ConfigurationErrorsException("Type not a specialization of " + baseType);
                }

                return type;
            }
        }
    }
}
