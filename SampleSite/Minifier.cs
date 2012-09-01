using Jacobsoft.Amd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahoo.Yui.Compressor;

namespace SampleSite
{
    public class Minifier : IScriptMinifier
    {
        public string Minify(string script)
        {
            var compressor = new JavaScriptCompressor();
            return compressor.Compress(script);
        }
    }
}