using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using Jacobsoft.Amd.Config;

namespace Jacobsoft.Amd.Internals.Config
{
    internal class AmdBundleConfigurationElement : ConfigurationElement, IBundle
    {
        public string Id { get; private set; }

        public IEnumerable<string> Modules { get; private set; }

        protected override void DeserializeElement(
            XmlReader reader, 
            bool serializeCollectionKey)
        {
            this.Id = reader.GetAttribute("id");
            this.Modules = reader
                .ReadElementContentAsString()
                .Split(new[] { ',', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();
        }
    }
}
