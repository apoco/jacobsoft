using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal static class ObjectExtensions
    {
        public static TValue IfExists<TObject, TValue>(
            this TObject obj,
            Func<TObject, TValue> valueDelegate)
        {
            if (obj == null)
            {
                return default(TValue);
            }

            return valueDelegate(obj);
        }
    }
}
