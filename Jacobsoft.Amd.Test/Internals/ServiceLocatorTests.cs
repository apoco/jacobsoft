using System;
using Jacobsoft.Amd.Config;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void GetConfiguration()
        {
            var factory = new ServiceLocator();
            Assert.IsNotNull(factory.Get<IAmdConfiguration>());
        }

        [TestMethod]
        public void GetModuleResolver()
        {
            var factory = new ServiceLocator();
            Assert.IsNotNull(factory.Get<IModuleResolver>());
        }

        [TestMethod]
        public void GetFileSystem()
        {
            var factory = new ServiceLocator();
            Assert.IsNotNull(factory.Get<IFileSystem>());
        }
    }
}
