using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Jacobsoft.Amd.Internals.Config;

namespace Jacobsoft.Amd.Config
{
    public class AmdShimCollectionConfigurationSection 
        : ConfigurationElementCollection, IEnumerable<IShim>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AmdShimConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as AmdShimConfigurationElement).Id;
        }

        IEnumerator<IShim> IEnumerable<IShim>.GetEnumerator()
        {
            return this.CastAllItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        private IEnumerable<IShim> CastAllItems()
        {
            var baseEnumerator = (this as IEnumerable).GetEnumerator();
            while (baseEnumerator.MoveNext())
            {
                yield return baseEnumerator.Current as IShim;
            }
        }
    }
}
