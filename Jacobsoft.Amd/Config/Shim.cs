using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Config
{
    /// <summary>
    /// Configuration for a module shim, which is a helper to ease integration of non-AMD scripts with an AMD system.
    /// </summary>
    public class Shim : IShim
    {
        /// <summary>
        /// The module ID to assign this shim.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// JavaScript expression to evaluate to calculate the shim's export (optional).
        /// </summary>
        public string Export { get; set; }

        /// <summary>
        /// Optional list of module ids representing the shim's dependencies
        /// </summary>
        public IEnumerable<string> Dependencies { get; set; }
    }
}
