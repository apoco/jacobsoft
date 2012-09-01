using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class ObjectExtensionsTests
    {
        [TestMethod]
        public void IfExists_WithNullObject()
        {
            TestObject obj = null;
            Assert.IsNull(obj.IfExists(o => o.SomeProperty));
        }

        [TestMethod]
        public void IfExists_WithNonNullObject()
        {
            TestObject obj = new TestObject();
            Assert.AreEqual("Foo", obj.IfExists(o => o.SomeProperty));
        }

        public class TestObject
        {
            public string SomeProperty
            {
                get { return "Foo"; }
            }
        }
    }
}
