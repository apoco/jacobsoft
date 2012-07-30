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
            config
                .Setup(c => c.LoaderUrl)
                .Returns("~/Scripts/require.js");
            AmdConfiguration.Current = config.Object;
        }

        [TestMethod]
        public void InvokeModule()
        {
            var html = this.htmlHelper.InvokeModule("module");
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/Scripts/require.js");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            this.AssertScriptInvoked(scripts[2], "module");
        }

        [TestMethod]
        public void InvokeModule_IncludesLoaderAndConfigOnlyOnce()
        {
            var html = this.htmlHelper.InvokeModule("a");
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/Scripts/require.js");
            this.AssertScriptInclude(scripts[1], "/amd/config");
            this.AssertScriptInvoked(scripts[2], "a");

            html = this.htmlHelper.InvokeModule("b");
            scripts = this.ExtractScriptTags(html);

            this.AssertScriptInvoked(scripts[0], "b");
        }

        [TestMethod]
        public void InvokeModule_WithOptions()
        {
            var html = this.htmlHelper.InvokeModule("module", new { key = "value" });
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/Scripts/require.js");
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
            var html = this.htmlHelper.InvokeModule(
                "module", 
                new Dictionary<string, object> { { "options", new { key = "value" } } });
            var scripts = this.ExtractScriptTags(html);

            this.AssertScriptInclude(scripts[0], "/Scripts/require.js");
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
