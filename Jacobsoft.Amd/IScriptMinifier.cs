using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd
{
    public interface IScriptMinifier
    {
        string Minify(string script);
    }
}
