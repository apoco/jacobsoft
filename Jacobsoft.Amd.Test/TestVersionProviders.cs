using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Test
{
    public class TestVersionProvider : IVersionProvider
    {
        public string GetVersion()
        {
            throw new NotImplementedException();
        }
    }

    public class StaticVersionProvider : IVersionProvider
    {
        private readonly string version;

        public StaticVersionProvider(string version)
        {
            this.version = version;
        }

        public string GetVersion()
        {
            return version;
        }
    }
}
