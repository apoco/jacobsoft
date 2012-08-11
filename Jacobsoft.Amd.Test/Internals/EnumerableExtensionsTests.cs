using System.Collections.Generic;
using System.Linq;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void OrEmpty_WithNonNullCollection()
        {
            var objects = new[] { 2, 3, 4 };
            Assert.AreEqual(objects, objects.OrEmpty());
        }

        [TestMethod]
        public void OrEmpty_WithNull()
        {
            IEnumerable<int> objects = null;
            Assert.IsNotNull(objects.OrEmpty());
            Assert.AreEqual(0, objects.OrEmpty().Count());
        }
    }
}
