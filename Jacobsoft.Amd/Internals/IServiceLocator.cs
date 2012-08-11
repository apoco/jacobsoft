using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal interface IServiceLocator
    {
        T Get<T>() where T : class;
    }
}
