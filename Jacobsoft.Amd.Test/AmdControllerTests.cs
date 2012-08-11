using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Antlr.Runtime;
using AutoMoq;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Internals;
using Jacobsoft.Amd.Internals.AntlrGenerated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jacobsoft.Amd.Test
{
    [TestClass]
    public class AmdControllerTests
    {
        private AutoMoqer autoMocker;
        private AmdController controller;
        private JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        [TestInitialize]
        public void Initialize()
        {
            this.autoMocker = new AutoMoqer();
            this.controller = new AmdController(
                this.autoMocker.GetMock<IAmdConfiguration>().Object,
                this.autoMocker.GetMock<IModuleResolver>().Object,
                this.autoMocker.GetMock<IFileSystem>().Object);
            
            var httpContext = this.autoMocker.GetMock<HttpContextBase>();
            httpContext
                .Setup(c => c.Request)
                .Returns(this.autoMocker.GetMock<HttpRequestBase>().Object);

            controller.ControllerContext = new ControllerContext(
                httpContext.Object,
                new RouteData(),
                controller);
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
                    .Setup(c => c.LoaderUrl)
                    .Returns(loaderPath);

                this.autoMocker
                    .GetMock<IFileSystem>()
                    .Setup(fs => fs.Open(loaderPath, FileMode.Open))
                    .Returns(loaderFileStream);

                var result = this.controller.Loader();
                Assert.IsInstanceOfType(result, typeof(FileStreamResult));

                Assert.AreEqual("text/javascript", result.ContentType);
                Assert.AreEqual(loaderFileStream, result.FileStream);
            }
        }

        [TestMethod]
        public void GetLiteLoader()
        {
            var result = this.controller.LiteLoader();
            Assert.AreEqual("text/javascript", result.ContentType);
        }

        [TestMethod]
        public void GetConfig()
        {
            autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns("~/Scripts");

            autoMocker
                .GetMock<HttpRequestBase>()
                .Setup(r => r.ApplicationPath)
                .Returns("/");

            var result = this.controller.Config() as ContentResult;
            Assert.IsNotNull(result);

            var program = JavaScriptTestHelper.ParseProgram(result.Content);
            var configCall = program.Statements[0].As<CallExpression>();

            var functionRef = configCall.Function.As<PropertyExpression>();
            functionRef.Object.Is<Identifier>("require");
            functionRef.Property.Is<Identifier>("config");

            var configVals = configCall.Arguments[0].As<ObjectLiteral>();
            var baseUrlLiteral = configVals
                .Assignments
                .Single(a => a.Property.Text == "\"baseUrl\"")
                .Value
                .As<StringLiteral>();
            var baseUrl = this.jsSerializer.Deserialize<string>(baseUrlLiteral.Text);
            Assert.AreEqual("/Scripts", baseUrl);
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

            var result = this.controller.Module(moduleName);

            Assert.AreEqual("text/javascript", result.ContentType);
            Assert.AreEqual(content, result.Content);
        }
    }
}
