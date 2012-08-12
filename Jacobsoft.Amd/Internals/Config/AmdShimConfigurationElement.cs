using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Config;

namespace Jacobsoft.Amd.Internals.Config
{
    internal class AmdShimConfigurationElement : ConfigurationElement, IShim
    {
        [ConfigurationProperty("id")]
        public string Id
        {
            get { return base["id"] as string; }
        }

        [ConfigurationProperty("export")]
        public string Export
        {
            get { return base["export"] as string; }
        }

        [ConfigurationProperty("dependencies")]
        [TypeConverter(typeof(StringListConverter))]
        public IEnumerable<string> Dependencies
        {
            get { return base["dependencies"] as IEnumerable<string>; }
        }

        private class StringListConverter : ConfigurationConverterBase
        {
            public override object ConvertFrom(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value)
            {
                var idList = value as string;
                return
                    from id in idList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    select id.Trim();
            }
        }
    }
}
