using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Antlr.Runtime;
using Jacobsoft.Amd.Config;
using Jacobsoft.Amd.Exceptions;
using Jacobsoft.Amd.Internals.AntlrGenerated;

namespace Jacobsoft.Amd.Internals
{
    internal class ModuleResolver : IModuleResolver
    {
        private readonly IAmdConfiguration config;
        private readonly IModuleRepository repository;
        private readonly HttpServerUtilityBase httpServer;
        private readonly IFileSystem fileSystem;

        private readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        public ModuleResolver(
            IAmdConfiguration config, 
            IModuleRepository repository,
            HttpServerUtilityBase httpServer,
            IFileSystem fileSystem)
        {
            this.config = config;
            this.repository = repository;
            this.httpServer = httpServer;
            this.fileSystem = fileSystem;
        }

        public IModule Resolve(string moduleId)
        {
            IModule module;
            if (!this.repository.TryGetModule(moduleId, out module))
            {
                var segments = this.GetModulePath(moduleId).ToList();
                module = this.Resolve(segments);
            }
            return module;
        }

        private IModule Resolve(IEnumerable<string> subFolders, string moduleId)
        {
            IList<string> absolutePath;

            var segments = this.GetModulePath(moduleId);

            var firstSegment = segments.First();
            absolutePath = (firstSegment == "." || firstSegment == "..")
                ? subFolders.ToList()
                : new List<string>();

            foreach (var segment in segments)
            {
                if (segment == "..")
                {
                    var indexToRemove = absolutePath.Count - 1;
                    if (indexToRemove < 0)
                    {
                        throw new InvalidPathException(
                            moduleId, 
                            "Dependency cannot refer to directory above the root module directory.");
                    }

                    absolutePath.RemoveAt(indexToRemove);
                }
                else if (segment != ".")
                {
                    absolutePath.Add(segment);
                }
            }

            return this.Resolve(absolutePath);
        }

        private IModule Resolve(IEnumerable<string> idSegments)
        {
            var normalizedId = string.Join("/", idSegments);
            var subFolders = idSegments.Take(idSegments.Count() - 1);

            if (this.config.Shims != null && this.config.Shims.ContainsKey(normalizedId))
            {
                return this.ResolveShimModule(subFolders, this.config.Shims[normalizedId]);
            }

            var filePath = this.GetModuleFileName(idSegments);
            if (!this.fileSystem.FileExists(filePath))
            {
                var module = new Module 
                { 
                    Id = normalizedId, 
                    Dependencies = Enumerable.Empty<IModule>()
                };
                this.repository.Add(module);
                return module;
            }

            using (var stream = this.fileSystem.Open(filePath, FileMode.Open))
            {
                var charStream = new ANTLRInputStream(stream);
                var lexer = new JavaScriptLexer(charStream);
                var tokenStream = new TokenRewriteStream(lexer);
                var parser = new JavaScriptParser(tokenStream);
                var program = parser.program().Tree as Program;

                var dependencies = new List<IModule>();

                var moduleDef = (
                    from callExpression in program.Children.OfType<CallExpression>()
                    where callExpression.Function.Text == "define" 
                    select callExpression
                ).SingleOrDefault();

                if (moduleDef != null)
                {
                    var argsToProcess = new List<Expression>(moduleDef.Arguments);
                    if (argsToProcess.Count == 0)
                    {
                        throw new InvalidModuleException(
                            normalizedId, 
                            "Module definition is empty.");
                    }
                    else if (argsToProcess.Count > 3)
                    {
                        throw new InvalidModuleException(
                            normalizedId,
                            "Module definition contains unexpected extra arguments.");
                    }

                    var factory = argsToProcess.Last();
                    argsToProcess.Remove(factory);

                    var depsArray = argsToProcess.LastOrDefault() as ArrayLiteral;
                    if (depsArray == null)
                    {
                        tokenStream.InsertBefore(factory.Token, "[], ");
                    }
                    else
                    {
                        foreach (var arrayItem in depsArray.Items)
                        {
                            if (arrayItem is StringLiteral)
                            {
                                var dependencyName =
                                    jsSerializer.Deserialize<string>(arrayItem.Text);
                                var dependency = this.Resolve(
                                    subFolders,
                                    dependencyName);
                                dependencies.Add(dependency);

                                tokenStream.Replace(
                                    (arrayItem as StringLiteral).Token.TokenIndex,
                                    string.Format("'{0}'", dependency.Id));
                            }
                        }
                        argsToProcess.Remove(depsArray);
                    }

                    var idLiteral = argsToProcess.LastOrDefault() as StringLiteral;
                    if (idLiteral == null)
                    {
                        tokenStream.InsertAfter(
                            moduleDef.ArgumentList.Token,
                            string.Format("'{0}', ", normalizedId));
                    }
                    else
                    {
                        var definedName = 
                            jsSerializer.Deserialize<string>(idLiteral.Text);
                        if (definedName != normalizedId)
                        {
                            throw new InvalidModuleException(
                                normalizedId, 
                                string.Format(
                                    "Module name '{0}' does not match expected value '{1}'. Anonymous modules are recommended.",
                                    definedName,
                                    normalizedId));
                        }
                    }
                }

                var module = new Module
                {
                    Id = normalizedId,
                    Content = tokenStream.ToString(),
                    Dependencies = dependencies
                };
                this.repository.Add(module);
                return module;
            }
        }

        private IModule ResolveShimModule(IEnumerable<string> subFolders, IShim shim)
        {
            var serializer = new JavaScriptSerializer();
            string content = null;

            var dependencies = shim
                .Dependencies
                .OrEmpty()
                .Select(d => this.Resolve(subFolders, d))
                .ToList();

            var fileName = this.GetModuleFileName(this.GetModulePath(shim.Id));
            if (this.fileSystem.FileExists(fileName))
            {
                using (var fileStream = this.fileSystem.Open(fileName, FileMode.Open))
                using (var reader = new StreamReader(fileStream))
                {
                    content = reader.ReadToEnd();
                    content = string.Format(
                        "define({0}, {1}, function() {{ {2}; return {3}; }});",
                        serializer.Serialize(shim.Id),
                        serializer.Serialize(dependencies.Select(d => d.Id)),
                        content,
                        shim.Export);
                }
            }

            return new Module
            {
                Id = shim.Id,
                Dependencies = dependencies,
                Content = content
            };
        }

        private string GetModuleFileName(IEnumerable<string> idSegments)
        {
            var filePath = idSegments.Aggregate(
                this.httpServer.MapPath(this.config.ModuleRootUrl),
                Path.Combine) + ".js";
            return filePath;
        }

        private IEnumerable<string> GetModulePath(string moduleId)
        {
            return moduleId.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
