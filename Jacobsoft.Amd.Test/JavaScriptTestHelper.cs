using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Jacobsoft.Amd.Internals.AntlrGenerated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test
{
    internal static class JavaScriptTestHelper
    {
        internal static Program ParseProgram(string code)
        {
            var charStream = new ANTLRStringStream(code);
            var tokenSource = new JavaScriptLexer(charStream);
            var tokenStream = new CommonTokenStream(tokenSource);
            var parser = new JavaScriptParser(tokenStream);
            var program = parser.program().Tree as Program;
            return program;
        }

        internal static T As<T>(this ITree node) where T : BaseNode
        {
            Assert.IsInstanceOfType(node, typeof(T));
            return (T)node;
        }

        internal static void Is<T>(this ITree node) where T : BaseNode
        {
            node.As<T>();
        }

        internal static void Is<T>(this ITree node, string text) where T : BaseNode
        {
            Assert.AreEqual(text, node.As<T>().Text);
        }
    }
}
