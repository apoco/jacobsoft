using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Antlr.Runtime;
using Jacobsoft.Amd.Exceptions;
using Jacobsoft.Amd.Internals.AntlrGenerated;

namespace Jacobsoft.Amd.Internals
{
    internal class ModuleResolver : IModuleResolver
    {
        private readonly IAmdConfiguration config;
        private readonly IModuleRepository repository;
        private readonly IFileSystem fileSystem;

        private readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        public ModuleResolver(
            IAmdConfiguration config, 
            IModuleRepository repository,
            IFileSystem fileSystem)
        {
            this.config = config;
            this.repository = repository;
            this.fileSystem = fileSystem;
        }

        public IModule Resolve(string moduleName)
        {
            IModule module;
            if (!this.repository.TryGetModule(moduleName, out module))
            {
                var segments = this.GetModulePath(moduleName).ToList();
                module = this.Resolve(segments);
                this.repository.Add(module);
            }
            return module;
        }

        private IModule Resolve(IEnumerable<string> subFolders, string moduleName)
        {
            var absolutePath = subFolders.ToList();

            var segments = this.GetModulePath(moduleName);
            foreach (var segment in segments)
            {
                if (segment == "..")
                {
                    var indexToRemove = absolutePath.Count - 1;
                    if (indexToRemove < 0)
                    {
                        throw new InvalidPathException(
                            moduleName, 
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

        private IModule Resolve(IEnumerable<string> nameSegments)
        {
            var normalizedName = string.Join("/", nameSegments);
            var filePath = nameSegments.Aggregate(this.config.ModuleRootUrl, Path.Combine) + ".js";
            var subFolders = nameSegments.Take(nameSegments.Count() - 1);

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
                            normalizedName, 
                            "Module definition is empty.");
                    }
                    else if (argsToProcess.Count > 3)
                    {
                        throw new InvalidModuleException(
                            normalizedName,
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
                                    string.Format("'{0}'", dependency.Name));
                            }
                        }
                        argsToProcess.Remove(depsArray);
                    }

                    var nameLiteral = argsToProcess.LastOrDefault() as StringLiteral;
                    if (nameLiteral == null)
                    {
                        tokenStream.InsertAfter(
                            moduleDef.ArgumentList.Token,
                            string.Format("'{0}', ", normalizedName));
                    }
                    else
                    {
                        var definedName = 
                            jsSerializer.Deserialize<string>(nameLiteral.Text);
                        if (definedName != normalizedName)
                        {
                            throw new InvalidModuleException(
                                normalizedName, 
                                string.Format(
                                    "Module name '{0}' does not match expected value '{1}'. Anonymous modules are recommended.",
                                    definedName,
                                    normalizedName));
                        }
                    }
                }

                return new Module
                {
                    Name = normalizedName,
                    Content = tokenStream.ToString(),
                    Dependencies = dependencies
                };
            }
        }

        private IEnumerable<string> GetModulePath(string moduleName)
        {
            return moduleName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
