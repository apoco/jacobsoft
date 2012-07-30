using System;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test
{
    [TestClass]
    public class AmdConfigurationTests
    {
        [TestMethod]
        public void Constructor_ReadsConfigFile()
        {
            var config = new AmdConfiguration();
            Assert.AreEqual("require.js", config.LoaderUrl);
            Assert.AreEqual("~/Scripts", config.ModuleRootUrl);
            Assert.IsTrue(config.IsOptimizationEnabled);
        }
    }
}
