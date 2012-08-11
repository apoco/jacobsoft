using System;
using System.Configuration;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test
{
    [TestClass]
    public class AmdConfigurationSectionTests
    {
        [TestMethod]
        public void LoaderUrl()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            Assert.AreEqual(@"require.js", configSection.LoaderUrl);
        }

        [TestMethod]
        public void ModuleFolder()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd") 
                as AmdConfigurationSection;
            Assert.AreEqual(@"~/Scripts", configSection.RootModuleUrl);
        }

        [TestMethod]
        public void Mode_WhenUnspecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            Assert.AreEqual(ScriptLoadingMode.Dynamic, configSection.ScriptLoadingMode);
        }

        [TestMethod]
        public void Mode_WhenDynamicSpecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.dynamic")
                as AmdConfigurationSection;
            Assert.AreEqual(ScriptLoadingMode.Dynamic, configSection.ScriptLoadingMode);
        }

        [TestMethod]
        public void Mode_WhenStaticSpecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.static")
                as AmdConfigurationSection;
            Assert.AreEqual(ScriptLoadingMode.Static, configSection.ScriptLoadingMode);
        }

        [TestMethod]
        public void Mode_WhenBundledSpecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.bundled")
                as AmdConfigurationSection;
            Assert.AreEqual(ScriptLoadingMode.Bundled, configSection.ScriptLoadingMode);
        }
    }
}
