using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Antlr.Runtime;
using AutoMoq;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Config;
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
            var request = this.autoMocker.GetMock<HttpRequestBase>();
            var server = this.autoMocker.GetMock<HttpServerUtilityBase>();

            httpContext.Setup(c => c.Request).Returns(request.Object);
            httpContext.Setup(c => c.Server).Returns(server.Object);
            request.Setup(r => r.ApplicationPath).Returns("/");

            controller.ControllerContext = new ControllerContext(
                httpContext.Object,
                new RouteData(),
                controller);
        }

        [TestMethod]
        public void RegisterRoutes()
        {
            var routes = new RouteCollection();
            AmdController.RegisterRoutes(routes);

            var route = routes["Jacobsoft.Amd"];
            Assert.IsNotNull(route);
        }

        [TestMethod]
        public void DefaultConstructor()
        {
            new AmdController();
        }

        [TestMethod]
        public void GetLoader()
        {
            var loaderVirtualPath = "~/Scripts/loader.js";
            var expectedPath = @"X:\loader.js";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.LoaderUrl)
                .Returns(loaderVirtualPath);

            this.autoMocker
                .GetMock<HttpServerUtilityBase>()
                .Setup(u => u.MapPath(loaderVirtualPath))
                .Returns(expectedPath);

            var result = this.controller.Loader();
            Assert.AreEqual("text/javascript", result.ContentType);
            Assert.AreEqual(expectedPath, result.FileName);
        }

        [TestMethod]
        public void GetLiteLoader()
        {
            var result = this.controller.LiteLoader();
            Assert.AreEqual("text/javascript", result.ContentType);
        }

        [TestMethod]
        public void GetConfig_SetsBaseUrl()
        {
            autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns("~/Scripts");

            var configVals = this.GetConfigObject();
            var baseUrlLiteral = configVals
                .Assignments
                .Single(a => a.Property.Text == "\"baseUrl\"")
                .Value
                .As<StringLiteral>();
            var baseUrl = this.jsSerializer.Deserialize<string>(baseUrlLiteral.Text);
            Assert.AreEqual("/Scripts", baseUrl);
        }

        [TestMethod]
        public void Config_SetsShims()
        {
            autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.Shims)
                .Returns(new Dictionary<string, IShim> { 
                    { 
                        "nonamd", 
                        new Shim 
                        { 
                            Id = "nonamd", 
                            Export = "foo",
                            Dependencies = new[] { "a", "b" } 
                        } 
                    }
                });

            var configObj = this.GetConfigObject();
            var shims = configObj
                .Assignments
                .Where(a => a.Property is StringLiteral)
                .Single(a => (a.Property as StringLiteral).String == "shim")
                .Value as ObjectLiteral;
            var shim = shims
                .Assignments
                .Single(a => (a.Property as StringLiteral).String == "nonamd")
                .Value as ObjectLiteral;

            Assert.AreEqual(
                "foo", 
                shim.Assignments
                    .Single(a => (a.Property as StringLiteral).String == "exports")
                    .Value
                    .As<StringLiteral>()
                    .String);
            Assert.IsTrue(
                shim.Assignments
                    .Single(a => (a.Property as StringLiteral).String == "deps")
                    .Value
                    .As<ArrayLiteral>()
                    .Items
                    .Cast<StringLiteral>()
                    .Select(lit => lit.String)
                    .SequenceEqual(new[] { "a", "b" }));
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

        [TestMethod]
        public void GetBundle()
        {
            var moduleA = Mock.Of<IModule>();
            var moduleB = Mock.Of<IModule>();
            var moduleD = Mock.Of<IModule>();

            Mock.Get(moduleA).Setup(m => m.Content).Returns("a");
            Mock.Get(moduleB).Setup(m => m.Content).Returns("b");
            Mock.Get(moduleD).Setup(m => m.Content).Returns("d");

            this.autoMocker
                .GetMock<IModuleResolver>()
                .Setup(r => r.Resolve("a"))
                .Returns(moduleA);
            this.autoMocker
                .GetMock<IModuleResolver>()
                .Setup(r => r.Resolve("b"))
                .Returns(moduleB);
            this.autoMocker
                .GetMock<IModuleResolver>()
                .Setup(r => r.Resolve("c/d"))
                .Returns(moduleD);

            var result = this.controller.Bundle("a+b+c/d");
            Assert.IsInstanceOfType(result, typeof(JavaScriptResult));
            Assert.AreEqual("a;b;d", (result as JavaScriptResult).Script);
        }

        private ObjectLiteral GetConfigObject()
        {
            var result = this.controller.Config();
            Assert.IsNotNull(result);

            var program = JavaScriptTestHelper.ParseProgram(result.Script);
            var configCall = program.Statements[0].As<CallExpression>();

            var functionRef = configCall.Function.As<PropertyExpression>();
            functionRef.Object.Is<Identifier>("require");
            functionRef.Property.Is<Identifier>("config");

            var configVals = configCall.Arguments[0].As<ObjectLiteral>();
            return configVals;
        }
    }
}
