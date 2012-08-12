using System;
using System.Reflection;
using System.Web;
using AutoMoq;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class AssemblyVersionProviderTests
    {
        private AutoMoqer autoMocker;

        [TestInitialize]
        public void Initialize()
        {
            this.autoMocker = new AutoMoqer();
        }

        [TestMethod]
        public void GetVersion()
        {
            var expectedVersion = this.GetType().Assembly.GetName().Version.ToString();

            this.autoMocker
                .GetMock<HttpContextBase>()
                .Setup(c => c.ApplicationInstance)
                .Returns(new TestApplication());

            var provider = this.autoMocker.Resolve<AssemblyVersionProvider>();
            Assert.AreEqual(expectedVersion, provider.GetVersion());
        }

        private class TestApplication : HttpApplication
        {
        }
    }
}
