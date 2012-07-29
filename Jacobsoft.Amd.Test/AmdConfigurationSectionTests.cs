using System;
using System.Configuration;
using AutoMoq.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test
{
    [TestClass]
    public class AmdConfigurationSectionTests
    {
        [TestMethod]
        public void ModuleFolder()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd") 
                as AmdConfigurationSection;
            Assert.AreEqual(@"~/Scripts", configSection.RootModuleUrl);
        }

        [TestMethod]
        public void IsOptimizationEnabled_WhenTrue()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            Assert.IsTrue(configSection.IsOptimizationEnabled);
        }

        [TestMethod]
        public void IsOptimizationEnabled_WhenFalse()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.nonOptimized")
                as AmdConfigurationSection;
            Assert.IsFalse(configSection.IsOptimizationEnabled);
        }

        [TestMethod]
        public void IsOptimizationEnabled_WhenUnspecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.noOptimizationSetting")
                as AmdConfigurationSection;
            Assert.IsFalse(configSection.IsOptimizationEnabled);
        }
    }
}
