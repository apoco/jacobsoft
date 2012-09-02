using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals.Config;

namespace Jacobsoft.Amd.Config
{
    public class AmdModuleBundleCollectionConfigurationSection
        : ConfigurationElementCollection, IEnumerable<IBundle>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AmdBundleConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as AmdBundleConfigurationElement).Id;
        }

        IEnumerator<IBundle> IEnumerable<IBundle>.GetEnumerator()
        {
            var baseEnumerator = base.GetEnumerator();
            while (baseEnumerator.MoveNext())
            {
                yield return (AmdBundleConfigurationElement)baseEnumerator.Current;
            }
        }
    }
}
