using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using AutoMoq;
using Jacobsoft.Amd.Config;
using Jacobsoft.Amd.Exceptions;
using Jacobsoft.Amd.Internals;
using Jacobsoft.Amd.Internals.AntlrGenerated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jacobsoft.Amd.Test
{
    [TestClass]
    public class HtmlHelperExtensionsTests
    {
        private AutoMoqer autoMocker;
        private HtmlHelper htmlHelper;

        [TestInitialize]
        public void Initialize()
        {
            this.autoMocker = new AutoMoqer();
            ServiceLocator.Instance = new MockServiceLocator(this.autoMocker);

            var request = autoMocker.GetMock<HttpRequestBase>();
            request.Setup(r => r.ApplicationPath).Returns("/");
            request.Setup(r => r.Url).Returns(new Uri("/", UriKind.Relative));
            request.Setup(x => x.ServerVariables).Returns(new NameValueCollection());

            var response = autoMocker.GetMock<HttpResponseBase>();
            response
                .Setup(r => r.ApplyAppPathModifier(It.IsAny<string>()))
                .Returns((string url) => url);

            var httpContext = autoMocker.GetMock<HttpContextBase>();
            httpContext.Setup(c => c.Items).Returns(new Dictionary<string, object>());
            httpContext.Setup(c => c.Request).Returns(request.Object);
            httpContext.Setup(c => c.Response).Returns(response.Object);

            var routes = RouteTable.Routes;
            routes.Clear();
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            this.htmlHelper = new HtmlHelper(
                new ViewContext(
                    new ControllerContext(
                        httpContext.Object,
                        new RouteData(),
                        autoMocker.GetMock<ControllerBase>().Object), 
                    autoMocker.GetMock<IView>().Object, 
                    new ViewDataDictionary(),
                    new TempDataDictionary(),
                    new StringWriter()),
                autoMocker.GetMock<IViewDataContainer>().Object);

            var config = autoMocker.GetMock<IAmdConfiguration>();
            config.Setup(c => c.LoaderUrl).Returns("~/Scripts/require.js");
        }

        [TestMethod]
        public void InvokeModule()
        {
            var html = this.htmlHelper.ModuleInvoke("module");
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/amd/loader");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            this.AssertScriptInvoked(scripts[2], "module");
        }

        [TestMethod]
        public void InvokeModule_IncludesLoaderAndConfigOnlyOnce()
        {
            var html = this.htmlHelper.ModuleInvoke("a");
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/amd/loader");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            this.AssertScriptInvoked(scripts[2], "a");

            html = this.htmlHelper.ModuleInvoke("b");
            scripts = this.ExtractScriptTags(html);

            this.AssertScriptInvoked(scripts[0], "b");
        }

        [TestMethod]
        public void InvokeModule_WithOptions()
        {
            var html = this.htmlHelper.ModuleInvoke("module", new { key = "value" });
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/amd/loader");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            
            var program = JavaScriptTestHelper.ParseProgram(scripts[2].Value);
            var optionsDef = program.Statements[0].As<CallExpression>();
            optionsDef.Function.Is<Identifier>("define");
            Assert.AreEqual("options", optionsDef.Arguments[0].As<StringLiteral>().String);
            Assert.AreEqual(0, optionsDef.Arguments[1].As<ArrayLiteral>().Items.Count);

            var options = optionsDef.Arguments[2].As<ObjectLiteral>();
            Assert.AreEqual("key", options.Assignments[0].Property.As<StringLiteral>().String);
            Assert.AreEqual("value", options.Assignments[0].Value.As<StringLiteral>().String);
        }

        [TestMethod]
        public void InvokeModule_WithModulesDictionary()
        {
            var html = this.htmlHelper.ModuleInvoke(
                "module", 
                new Dictionary<string, object> { { "options", new { key = "value" } } });
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/amd/loader");
            this.AssertScriptInclude(scripts[1], "/amd/config");

            var program = JavaScriptTestHelper.ParseProgram(scripts[2].Value);
            var optionsDef = program.Statements[0].As<CallExpression>();
            optionsDef.Function.Is<Identifier>("define");
            Assert.AreEqual("options", optionsDef.Arguments[0].As<StringLiteral>().String);
            Assert.AreEqual(0, optionsDef.Arguments[1].As<ArrayLiteral>().Items.Count);

            var options = optionsDef.Arguments[2].As<ObjectLiteral>();
            Assert.AreEqual("key", options.Assignments[0].Property.As<StringLiteral>().String);
            Assert.AreEqual("value", options.Assignments[0].Value.As<StringLiteral>().String);
        }

        [TestMethod, ExpectedException(typeof(AmdConfigurationException))]
        public void InvokeModule_WithStaticLoadingAndNoLoader_ThrowsConfigurationException()
        {
            var config = this.autoMocker.GetMock<IAmdConfiguration>();

            config.Setup(c => c.ScriptLoadingMode).Returns(ScriptLoadingMode.Dynamic);
            config.Setup(c => c.LoaderUrl).Returns((string)null);

            this.htmlHelper.ModuleInvoke("a");
        }

        [TestMethod]
        public void InvokeModule_WithStaticLoading()
        {
            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ScriptLoadingMode)
                .Returns(ScriptLoadingMode.Static);

            var html = this.htmlHelper.ModuleInvoke("a");
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/amd/liteloader");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            this.AssertScriptInclude(scripts[2], "/amd/module/a");
            this.AssertScriptInvoked(scripts[3], "a");
        }

        [TestMethod]
        public void InvokeModule_WithStaticLoading_InsertsDependencies()
        {
            var module = Mock.Of<IModule>();
            var dependency = Mock.Of<IModule>();

            Mock.Get(module).Setup(m => m.Dependencies).Returns(new[] { dependency });
            Mock.Get(module).Setup(m => m.Content).Returns("non empty");
            
            Mock.Get(dependency).Setup(m => m.Id).Returns("b");
            Mock.Get(dependency).Setup(m => m.Content).Returns("non empty");

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ScriptLoadingMode)
                .Returns(ScriptLoadingMode.Static);

            this.autoMocker
                .GetMock<IModuleResolver>()
                .Setup(r => r.Resolve("a"))
                .Returns(module);

            var html = this.htmlHelper.ModuleInvoke("a");
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/amd/liteloader");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            this.AssertScriptInclude(scripts[2], "/amd/module/b");
            this.AssertScriptInclude(scripts[3], "/amd/module/a");
            this.AssertScriptInvoked(scripts[4], "a");
        }

        [TestMethod]
        public void ModuleBundle()
        {
            var moduleA = Mock.Of<IModule>();
            var moduleC = Mock.Of<IModule>();
            var moduleD = Mock.Of<IModule>();

            var resolver = this.autoMocker.GetMock<IModuleResolver>();
            resolver.Setup(r => r.Resolve("a")).Returns(moduleA);
            resolver.Setup(r => r.Resolve("b/c")).Returns(moduleC);
            resolver.Setup(r => r.Resolve("b/d")).Returns(moduleD);

            Mock.Get(moduleA).Setup(m => m.Id).Returns("a");
            Mock.Get(moduleC).Setup(m => m.Id).Returns("b/c");
            Mock.Get(moduleD).Setup(m => m.Id).Returns("b/d");

            Mock.Get(moduleA).Setup(m => m.Dependencies).Returns(new[] { moduleC });
            Mock.Get(moduleC).Setup(m => m.Dependencies).Returns(new[] { moduleD });

            var html = this.htmlHelper.ModuleBundle("a");
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/amd/loader");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            this.AssertScriptInclude(scripts[2], "/amd/bundle/a%2cb/c%2cb/d");
        }

        private IList<XElement> ExtractScriptTags(IHtmlString html)
        {
            // To help us inspect the HTML, we will parse as XML.
            var root = XDocument.Parse("<root>" + html.ToHtmlString() + "</root>").Root;
            var scripts = root.Elements().ToList();
            return scripts;
        }

        private void AssertScriptInclude(XElement include, string src)
        {
            Assert.AreEqual("script", include.Name);
            Assert.IsTrue(include.Attribute("src").Value.Equals(
                src, 
                StringComparison.InvariantCultureIgnoreCase));
        }

        private void AssertScriptInvoked(XElement scriptElement, string moduleName)
        {
            var invokeScript = JavaScriptTestHelper.ParseProgram(scriptElement.Value);
            var call = invokeScript.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("require");

            var modules = call.Arguments[0].As<ArrayLiteral>();
            Assert.AreEqual(moduleName, modules.Items[0].As<StringLiteral>().String);
        }
    }
}
