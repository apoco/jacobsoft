using System;
using System.Linq;
using AutoMoq;
using Jacobsoft.Amd.Config;
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
            
            Assert.IsNotNull(config.Shims);

            var shims = config.Shims;
            Assert.AreEqual(3, shims.Count);
            
            Assert.AreEqual("foo", shims["foo"].Id);
            Assert.AreEqual("bar", shims["foo"].Export);
            Assert.IsTrue(shims["foo"].Dependencies.SequenceEqual(new[] { "baz", "bat" }));

            Assert.AreEqual("baz", shims["baz"].Id);
            Assert.AreEqual("baz", shims["baz"].Export);
            Assert.IsTrue(shims["baz"].Dependencies.SequenceEqual(new[] { "bat" }));

            Assert.AreEqual("bat", shims["bat"].Id);
            Assert.IsTrue(string.IsNullOrEmpty(shims["bat"].Export));
            Assert.IsNull(shims["bat"].Dependencies);
        }
    }
}
