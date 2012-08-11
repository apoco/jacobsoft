using System;
using AutoMoq;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class ModuleRepositoryTests
    {
        private AutoMoqer autoMocker;

        [TestInitialize]
        public void Initialize()
        {
            this.autoMocker = new AutoMoqer();
        }

        [TestMethod]
        public void Add()
        {
            var moduleName = "module";
            var module = Mock.Of<IModule>();

            Mock.Get(module)
                .Setup(m => m.Id)
                .Returns(moduleName);

            var repo = this.autoMocker.Resolve<ModuleRepository>();
            repo.Add(module);

            IModule module2;
            Assert.IsTrue(repo.TryGetModule(moduleName, out module2));
            Assert.AreEqual(module, module2);
        }
    }
}
