using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jacobsoft.Amd.Internals
{
    internal class AssemblyVersionProvider : IVersionProvider
    {
        private string version;

        public AssemblyVersionProvider(HttpContextBase httpContext)
        {
            this.version = httpContext
                .IfExists(ctx => ctx.ApplicationInstance)
                .IfExists(i => i.GetType().BaseType.Assembly.GetName().Version.ToString());
        }

        public string GetVersion()
        {
            return this.version;
        }
    }
}
