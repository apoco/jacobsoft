using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jacobsoft.Amd.Internals
{
    internal class AssemblyVersionProvider : IVersionProvider
    {
        private readonly Lazy<string> version;

        public AssemblyVersionProvider(HttpContextBase httpContext)
        {
            this.version = new Lazy<string>(() => httpContext
                .ApplicationInstance
                .GetType()
                .BaseType
                .Assembly
                .GetName()
                .Version
                .ToString());
        }

        public string GetVersion()
        {
            return this.version.Value;
        }
    }
}
