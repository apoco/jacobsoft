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
        public void Constructor_WithNoConfigFile()
        {
            var config = new AmdConfiguration(null, Mock.Of<IVersionProvider>());
            Assert.AreEqual(0, config.Shims.Count);
            Assert.AreEqual(0, config.Bundles.Count);
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

        [TestMethod]
        public void Constructor_WithBundles()
        {
            var bundleA = new Mock<IBundle>();
            bundleA.Setup(b => b.Id).Returns("bundleA");
            bundleA.Setup(b => b.Modules).Returns(new[] { "a", "b", "c" });

            var bundleB = new Mock<IBundle>();
            bundleB.Setup(b => b.Id).Returns("bundleB");
            bundleB.Setup(b => b.Modules).Returns(new[] { "d", "e" });

            var configSection = this.mocker.GetMock<IAmdConfigurationSection>();
            configSection
                .Setup(s => s.Bundles)
                .Returns(new[] { bundleA.Object, bundleB.Object });

            var config = this.GetConfiguration();
            Assert.IsTrue(config.Bundles.ContainsKey("bundleA"));
            Assert.IsTrue(config.Bundles["bundleA"].SequenceEqual(new[] { "a", "b", "c" }));
            Assert.IsTrue(config.Bundles.ContainsKey("bundleB"));
            Assert.IsTrue(config.Bundles["bundleB"].SequenceEqual(new[] { "d", "e" }));
        }

        private AmdConfiguration GetConfiguration()
        {
            return new AmdConfiguration(
                this.mocker.GetMock<IAmdConfigurationSection>().Object,
                this.mocker.GetMock<IVersionProvider>().Object);
        }
    }
}
