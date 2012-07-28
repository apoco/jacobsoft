using System;
using System.IO;
using System.Web.Mvc;
using AutoMoq;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jacobsoft.Amd.Test
{
    [TestClass]
    public class AmdControllerTests
    {
        private AutoMoqer autoMocker;
        private AmdController controller;

        [TestInitialize]
        public void Initialize()
        {
            this.autoMocker = new AutoMoqer();
            this.controller = new AmdController(
                this.autoMocker.GetMock<IAmdConfiguration>().Object,
                this.autoMocker.GetMock<IModuleResolver>().Object,
                this.autoMocker.GetMock<IFileSystem>().Object);
        }

        [TestMethod]
        public void DefaultConstructor()
        {
            new AmdController();
        }

        [TestMethod]
        public void GetLoader()
        {
            var loaderPath = @"X:\loader.js";
            var loaderContent = "Script content";

            using (var loaderFileStream = new MemoryStream())
            using (var writer = new StreamWriter(loaderFileStream))
            {
                writer.Write(loaderContent);

                this.autoMocker
                    .GetMock<IAmdConfiguration>()
                    .Setup(c => c.LoaderFilePath)
                    .Returns(loaderPath);

                this.autoMocker
                    .GetMock<IFileSystem>()
                    .Setup(fs => fs.Open(loaderPath, FileMode.Open))
                    .Returns(loaderFileStream);

                var result = this.controller.GetLoader();
                Assert.IsInstanceOfType(result, typeof(FileStreamResult));

                Assert.AreEqual("text/javascript", result.ContentType);
                Assert.AreEqual(loaderFileStream, result.FileStream);
            }
        }

        [TestMethod]
        public void GetModule()
        {
            var moduleName = "module";
            var content = "Module content";

            var module = Mock.Of<IModule>();

            this.autoMocker
                .GetMock<IModuleResolver>()
                .Setup(r => r.Resolve(moduleName))
                .Returns(module);

            Mock.Get(module)
                .Setup(m => m.Content)
                .Returns(content);

            var result = this.controller.GetModule(moduleName);

            Assert.AreEqual("text/javascript", result.ContentType);
            Assert.AreEqual(content, result.Content);
        }
    }
}
