using Jacobsoft.Amd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleSite
{
    public class VersionProvider : IVersionProvider
    {
        public string GetVersion()
        {
            return "Beta";
        }
    }
}