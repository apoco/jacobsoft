using System;
using System.Linq;
using AutoMoq;
using Jacobsoft.Amd.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jacobsoft.Amd.Internals;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Internals.Config;
using Moq;

namespace Jacobsoft.Amd.Test
{
    [TestClass]
    public class AmdConfigurationTests
    {
        private AutoMoqer mocker;

        [TestInitialize]
        public void Initialize()
        {
            this.mocker = new AutoMoqer();
        }

        [TestMethod]
        public void Constructor_ReadsConfigFile()
        {
            var expectedLoaderUrl = "require.js";
            var expectedModuleRootUrl = "~/Scripts";
            
            var configSection = this.mocker.GetMock<IAmdConfigurationSection>();
            configSection.Setup(s => s.LoaderUrl).Returns(expectedLoaderUrl);
            configSection.Setup(s => s.RootModuleUrl).Returns(expectedModuleRootUrl);
            configSection.Setup(s => s.Shims).Returns(new[] {
                new Shim { Id = "foo" },
                new Shim { Id = "bar" },
                new Shim { Id = "baz" }
            });

            var config = this.GetConfiguration();

            Assert.AreEqual(expectedLoaderUrl, config.LoaderUrl);
            Assert.AreEqual(expectedModuleRootUrl, config.ModuleRootUrl);
            Assert.IsNotNull(config.Shims);
            Assert.IsTrue(config.Shims.ContainsKey("foo"));
            Assert.IsTrue(config.Shims.ContainsKey("bar"));
            Assert.IsTrue(config.Shims.ContainsKey("baz"));
        }

        [TestMethod]
        public void Constructor_WithNoVersionProviderInConfigSection_UsesLocatorValue()
        {
            var configSection = this.mocker.GetMock<IAmdConfigurationSection>();
            configSection.Setup(s => s.VersionProvider).Returns((Type)null);
            
            var config = this.GetConfiguration();
            Assert.AreEqual(
                this.mocker.GetMock<IVersionProvider>().Object, 
                config.VersionProvider);
        }

        [TestMethod]
        public void Constructor_WithVersionProviderInConfigSection_ActivatesObject()
        {
            var expectedVersionProviderType = typeof(TestVersionProvider);

            var configSection = this.mocker.GetMock<IAmdConfigurationSection>();
            configSection.Setup(s => s.VersionProvider).Returns(expectedVersionProviderType);

            var config = this.GetConfiguration();
            Assert.IsInstanceOfType(config.VersionProvider, expectedVersionProviderType);
        }

        [TestMethod]
        public void Constructor_WithMinifierInConfigSection_ActivatesObject()
        {
            var expectedMinifierType = typeof(TestMinifier);

            var configSection = this.mocker.GetMock<IAmdConfigurationSection>();
            configSection.Setup(s => s.Minifier).Returns(expectedMinifierType);

            var config = this.GetConfiguration();
            Assert.IsInstanceOfType(config.Minifier, expectedMinifierType);
        }

        private AmdConfiguration GetConfiguration()
        {
            return new AmdConfiguration(
                this.mocker.GetMock<IAmdConfigurationSection>().Object,
                this.mocker.GetMock<IVersionProvider>().Object);
        }
    }
}
