using System;
using System.Configuration;
using System.Linq;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Config;
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

        [TestMethod]
        public void VersionProvider_WhenUnspecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            Assert.IsNull(configSection.VersionProvider);
        }

        [TestMethod]
        public void VersionProvider_WhenSpecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.versioning")
                as AmdConfigurationSection;
            Assert.AreEqual(typeof(TestVersionProvider), configSection.VersionProvider);
        }

        [TestMethod, ExpectedException(typeof(ConfigurationErrorsException))]
        public void VersionProvider_WhenNoAssemblySpecified()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.versioning.noassembly")
                as AmdConfigurationSection;
        }

        [TestMethod, ExpectedException(typeof(ConfigurationErrorsException))]
        public void VersionProvider_WhenAssemblyNotFound()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.versioning.assemblynotfound")
                as AmdConfigurationSection;
        }

        [TestMethod, ExpectedException(typeof(ConfigurationErrorsException))]
        public void VersionProvider_WhenTypeNotFound()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.versioning.typenotfound")
                as AmdConfigurationSection;
        }

        [TestMethod, ExpectedException(typeof(ConfigurationErrorsException))]
        public void VersionProvider_WhenTypeNotIVersionProviderImplementation()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.versioning.incompatible")
                as AmdConfigurationSection;
        }

        [TestMethod]
        public void Minifier()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd.minifying")
                as AmdConfigurationSection;
            Assert.AreEqual(typeof(TestMinifier), configSection.Minifier);
        }

        [TestMethod]
        public void Shims()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            var shims = configSection.Shims.ToList();

            Assert.AreEqual("foo", shims[0].Id);
            Assert.AreEqual("bar", shims[0].Export);
            Assert.IsTrue(shims[0].Dependencies.SequenceEqual(new[] { "baz", "bat" }));

            Assert.AreEqual("baz", shims[1].Id);
            Assert.AreEqual("baz", shims[1].Export);
            Assert.IsTrue(shims[1].Dependencies.SequenceEqual(new[] { "bat" }));

            Assert.AreEqual("bat", shims[2].Id);
        }

        [TestMethod]
        public void Bundles()
        {
            var configSection = ConfigurationManager.GetSection("jacobsoft.amd")
                as AmdConfigurationSection;
            var bundles = configSection.Bundles.ToList();

            Assert.AreEqual(2, bundles.Count);

            Assert.AreEqual("bundleA", bundles[0].Id);
            Assert.IsTrue(bundles[0].Modules.SequenceEqual(new[] { "a", "b", "c" }));

            Assert.AreEqual("bundleB", bundles[1].Id);
            Assert.IsTrue(bundles[1].Modules.SequenceEqual(new[] { "d", "e" }));
        }
    }
}
