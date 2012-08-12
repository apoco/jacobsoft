using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using AutoMoq;
using AutoMoq.Helpers;
using Jacobsoft.Amd.Config;
using Jacobsoft.Amd.Exceptions;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class ModuleResolverTests
    {
        private AutoMoqer autoMocker;

        [TestInitialize]
        public void Initialize()
        {
            this.autoMocker = new AutoMoqer();
            this.autoMocker
                .GetMock<HttpServerUtilityBase>()
                .Setup(s => s.MapPath("~/Scripts"))
                .Returns("X:\\Modules");
        }

        [TestMethod, ExpectedException(typeof(InvalidModuleException))]
        public void Resolve_WithEmptyModuleDefinition()
        {
            var moduleName = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define();");

            this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
        }

        [TestMethod, ExpectedException(typeof(InvalidModuleException))]
        public void Resolve_WithUnexpectedArgumentCountInDefinition()
        {
            var moduleName = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                @"define('module', ['foo', 'bar'], { some: 'value' }, function() { alert('Yo!'); });");

            this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
        }

        [TestMethod]
        public void Resolve_WithNameAlreadyDefined()
        {
            var moduleId = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define('module', function() { return 23; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleId);
            Assert.AreEqual("module", module.Id);
            Assert.AreEqual("define('module', [], function() { return 23; });", module.Content);
        }

        [TestMethod, ExpectedException(typeof(InvalidModuleException))]
        public void Resolve_InvalidNameDefined()
        {
            var moduleName = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define('foo', function() { return 23; });");

            this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
        }

        [TestMethod]
        public void Resolve_SetsModuleName()
        {
            var moduleName = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define(function() { return 23; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
            Assert.AreEqual("module", module.Id);
            Assert.AreEqual("define('module', [], function() { return 23; });", module.Content);
        }

        [TestMethod]
        public void Resolve_InsertsMissingDependencyArray()
        {
            var moduleName = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define(function() { return 23; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
            Assert.AreEqual("module", module.Id);
            Assert.AreEqual("define('module', [], function() { return 23; });", module.Content);
        }

        [TestMethod]
        public void Resolve_ResolvesDependencies()
        {
            var moduleName = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define(['foo', 'bar'], function(foo, bar) { return foo(bar); });");
            this.ArrangeJavaScriptFile(
                @"X:\Modules\foo.js",
                "define(function(){ return function(str) { window.alert(str); }; });");
            this.ArrangeJavaScriptFile(
                @"X:\Modules\bar.js",
                "define(function(){ return 'bar'; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
            var dependencies = module.Dependencies.ToList();
            Assert.AreEqual(2, dependencies.Count());

            Assert.AreEqual("foo", dependencies[0].Id);
            Assert.AreEqual(
                "define('foo', [], function(){ return function(str) { window.alert(str); }; });", 
                dependencies[0].Content);

            Assert.AreEqual("bar", dependencies[1].Id);
            Assert.AreEqual(
                "define('bar', [], function(){ return 'bar'; });",
                dependencies[1].Content);
        }

        [TestMethod]
        public void Resolve_HandlesCurrentDirectoryNames()
        {
            var moduleName = "library/module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\library\module.js",
                "define(['./foo'], function(foo) { return foo('bar'); });");
            this.ArrangeJavaScriptFile(
                @"X:\Modules\library\foo.js",
                "define(function(){ return function(str) { window.alert(str); }; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
            var dependencies = module.Dependencies.ToList();
            Assert.AreEqual(1, dependencies.Count());

            Assert.AreEqual("library/foo", dependencies[0].Id);
            Assert.AreEqual(
                "define('library/foo', [], function(){ return function(str) { window.alert(str); }; });",
                dependencies[0].Content);
        }

        [TestMethod]
        public void Resolve_HandlesAbsoluteDirectoryNames()
        {
            var moduleName = "library/module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\library\module.js",
                "define(['library/foo'], function(foo) { return foo('bar'); });");
            this.ArrangeJavaScriptFile(
                @"X:\Modules\library\foo.js",
                "define(function(){ return function(str) { window.alert(str); }; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
            var dependencies = module.Dependencies.ToList();
            Assert.AreEqual(1, dependencies.Count());

            Assert.AreEqual("library/foo", dependencies[0].Id);
            Assert.AreEqual(
                "define('library/foo', [], function(){ return function(str) { window.alert(str); }; });",
                dependencies[0].Content);
        }

        [TestMethod]
        public void Resolve_HandlesParentDirectoryNames()
        {
            var moduleName = "library/module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\library\module.js",
                "define(['../foo'], function(foo) { return foo('bar'); });");
            this.ArrangeJavaScriptFile(
                @"X:\Modules\foo.js",
                "define(function(){ return function(str) { window.alert(str); }; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
            var dependencies = module.Dependencies.ToList();
            Assert.AreEqual(1, dependencies.Count());

            Assert.AreEqual("foo", dependencies[0].Id);
            Assert.AreEqual(
                "define('foo', [], function(){ return function(str) { window.alert(str); }; });",
                dependencies[0].Content);
        }

        [TestMethod, ExpectedException(typeof(InvalidPathException))]
        public void Resolve_HandlesInvalidParentDirectoryNames()
        {
            var moduleName = "module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define(['../foo'], function(foo) { return foo('bar'); });");

            this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
        }

        [TestMethod]
        public void Resolve_HandlesChildDirectoryNames()
        {
            var moduleName = "library/module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\library\module.js",
                "define(['./subdir/foo'], function(foo) { return foo('bar'); });");
            this.ArrangeJavaScriptFile(
                @"X:\Modules\library\subdir\foo.js",
                "define(function(){ return function(str) { window.alert(str); }; });");

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);
            var dependencies = module.Dependencies.ToList();
            Assert.AreEqual(1, dependencies.Count());

            Assert.AreEqual("library/subdir/foo", dependencies[0].Id);
            Assert.AreEqual(
                "define('library/subdir/foo', [], function(){ return function(str) { window.alert(str); }; });",
                dependencies[0].Content);
        }

        [TestMethod]
        public void Resolve_HandlesNonModuleFiles()
        {
            var moduleName = "library/module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            var content = "foo = { };";
            this.ArrangeJavaScriptFile(@"X:\Modules\library\module.js", content);

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);

            Assert.AreEqual(moduleName, module.Id);
            Assert.AreEqual(content, module.Content);
            Assert.IsFalse(module.Dependencies.Any());
        }

        [TestMethod]
        public void Resolve_HandlesNotFoundDependencies()
        {
            var moduleName = "library/module";

            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            var content = "define(['./foo'], function(foo) { });";
            this.ArrangeJavaScriptFile(@"X:\Modules\library\module.js", content);

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleName);

            var dependencies = module.Dependencies.ToList();
            Assert.AreEqual("library/foo", dependencies[0].Id);
        }

        [TestMethod]
        public void Resolve_UsesShims()
        {
            var moduleId = "nonamd";
            var content = "dummyContent();";

            var config = this.autoMocker.GetMock<IAmdConfiguration>();
            config
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");
            config
                .Setup(c => c.Shims)
                .Returns(new Dictionary<string, IShim> { 
                    { 
                        moduleId,
                        new Shim { 
                            Id = moduleId, 
                            Dependencies = new[] { "a", "b" }, 
                            Export = "foo" } 
                    }
                });

            this.ArrangeJavaScriptFile(@"X:\Modules\nonamd.js", content);

            var module = this.autoMocker.Resolve<ModuleResolver>().Resolve(moduleId);
            Assert.AreEqual(moduleId, module.Id);
            Assert.IsTrue(
                module
                    .Dependencies
                    .Select(m => m.Id)
                    .SequenceEqual(new[] { "a", "b" }));
            Assert.AreEqual(content, module.Content);            
        }

        [TestMethod]
        public void Resolve_CachesResults()
        {
            this.autoMocker
                .GetMock<IAmdConfiguration>()
                .Setup(c => c.ModuleRootUrl)
                .Returns(@"~/Scripts");

            this.ArrangeJavaScriptFile(
                @"X:\Modules\module.js",
                "define(function() { return 'foo'; });");

            var resolver = this.autoMocker.Resolve<ModuleResolver>();
            var module = resolver.Resolve("module");

            this.autoMocker
                .GetMock<IModuleRepository>()
                .Verify(r => r.Add(module));
        }

        [TestMethod]
        public void Resolve_UsesCachedResults()
        {
            var moduleName = "module";
            var expectedModule = Mock.Of<IModule>();

            //var fromRepo = 
            this.autoMocker
                .GetMock<IModuleRepository>()
                .Setup(r => r.TryGetModule(moduleName, out expectedModule))
                .Returns(true);

            var resolver = this.autoMocker.Resolve<ModuleResolver>();
            var resolvedModule = resolver.Resolve("module");
            Assert.AreSame(expectedModule, resolvedModule);
        }

        private void ArrangeJavaScriptFile(string fileName, string fileContents)
        {
            var fileSystem = this.autoMocker.GetMock<IFileSystem>();

            fileSystem
                .Setup(fs => fs.FileExists(fileName))
                .Returns(true);
            fileSystem
                .Setup(fs => fs.Open(fileName, FileMode.Open))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(fileContents)));
        }
    }
}
