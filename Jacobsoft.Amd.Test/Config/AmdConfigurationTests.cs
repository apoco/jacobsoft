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
        [TestMethod]
        public void Constructor_ReadsConfigFile()
        {
            var expectedLoaderUrl = "require.js";
            var expectedModuleRootUrl = "~/Scripts";
            
            var configSection = new Mock<IAmdConfigurationSection>();
            configSection.Setup(s => s.LoaderUrl).Returns(expectedLoaderUrl);
            configSection.Setup(s => s.RootModuleUrl).Returns(expectedModuleRootUrl);
            configSection.Setup(s => s.Shims).Returns(new[] {
                new Shim { Id = "foo" },
                new Shim { Id = "bar" },
                new Shim { Id = "baz" }
            });

            var config = new AmdConfiguration(configSection.Object);

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
            var configSection = new Mock<IAmdConfigurationSection>();
            configSection.Setup(s => s.VersionProvider).Returns((Type)null);

            var versionProvider = Mock.Of<IVersionProvider>();

            var serviceLocator = new Mock<IServiceLocator>();
            serviceLocator.Setup(sl => sl.Get<IVersionProvider>()).Returns(versionProvider);
            ServiceLocator.Instance = serviceLocator.Object;

            var config = new AmdConfiguration(configSection.Object);
            Assert.AreEqual(versionProvider, config.VersionProvider);
        }

        [TestMethod]
        public void Constructor_WithVersionProviderInConfigSection_ActivatesObject()
        {
            var expectedVersionProviderType = typeof(TestVersionProvider);

            var configSection = new Mock<IAmdConfigurationSection>();
            configSection.Setup(s => s.VersionProvider).Returns(expectedVersionProviderType);

            var config = new AmdConfiguration(configSection.Object);
            Assert.IsInstanceOfType(config.VersionProvider, expectedVersionProviderType);
        }
    }
}
