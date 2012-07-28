using System;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class ObjectFactoryTests
    {
        [TestMethod]
        public void GetConfiguration()
        {
            var factory = new ObjectFactory();
            Assert.IsNotNull(factory.GetConfiguration());
        }

        [TestMethod]
        public void GetModuleResolver()
        {
            var factory = new ObjectFactory();
            Assert.IsNotNull(factory.GetModuleResolver());
        }

        [TestMethod]
        public void GetFileSystem()
        {
            var factory = new ObjectFactory();
            Assert.IsNotNull(factory.GetFileSystem());
        }
    }
}
