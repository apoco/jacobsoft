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
        private readonly IFileSystem fileSystem;

        private readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        public ModuleResolver(IAmdConfiguration config, IFileSystem fileSystem)
        {
            this.config = config;
            this.fileSystem = fileSystem;
        }

        public IModule Resolve(string moduleName)
        {
            var segments = this.GetModulePath(moduleName).ToList();
            return Resolve(segments);
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
            var filePath = nameSegments.Aggregate(this.config.ModuleFolder, Path.Combine) + ".js";
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
                    where callExpression.Children[0].Text == "define"
                    select callExpression
                ).SingleOrDefault();

                if (moduleDef != null)
                {
                    var i = 0;

                    if (moduleDef.Arguments[i] is StringLiteral)
                    {
                        var definedName = 
                            jsSerializer.Deserialize<string>(moduleDef.Arguments[i].Text);
                        if (definedName != normalizedName)
                        {
                            throw new InvalidNamedModuleException(
                                normalizedName, 
                                string.Format(
                                    "Module name '{0}' does not match expected value '{1}'. Anonymous modules are recommended.",
                                    definedName,
                                    normalizedName));
                        }
                        i++;
                    }
                    else
                    {
                        tokenStream.InsertBefore(
                            moduleDef.Arguments[i].Token.TokenIndex,
                            string.Format("'{0}', ", normalizedName));
                    }

                    if (moduleDef.Arguments[i] is ArrayLiteral)
                    {
                        foreach (var arrayItem in (moduleDef.Arguments[i] as ArrayLiteral).Children)
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
