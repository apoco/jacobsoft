using System.Collections.Generic;
using System.Linq;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void OrEmpty_WithNonNullCollection()
        {
            var objects = new[] { 2, 3, 4 };
            Assert.AreEqual(objects, objects.OrEmpty());
        }

        [TestMethod]
        public void OrEmpty_WithNull()
        {
            IEnumerable<int> objects = null;
            Assert.IsNotNull(objects.OrEmpty());
            Assert.AreEqual(0, objects.OrEmpty().Count());
        }

        [TestMethod]
        public void Descendents()
        {
            var tree = new TreeNode("a",
                new TreeNode("b"),
                new TreeNode("c",
                    new TreeNode("d"),
                    new TreeNode("e"),
                new TreeNode("f")));
            Assert.IsTrue(
                tree.Descendents(n => n.Children)
                    .Select(n => n.Label)
                    .SequenceEqual(new[] { "b", "c", "d", "e", "f" }));
        }

        [TestMethod]
        public void TreeNodes()
        {
            var tree = new TreeNode("a",
                new TreeNode("b"),
                new TreeNode("c",
                    new TreeNode("d"),
                    new TreeNode("e"),
                new TreeNode("f")));
            Assert.IsTrue(
                tree.TreeNodes(n => n.Children)
                    .Select(n => n.Label)
                    .SequenceEqual(new[] { "a", "b", "c", "d", "e", "f" }));
        }

        private class TreeNode
        {
            public TreeNode(string label, params TreeNode[] children)
            {
                this.Label = label;
                this.Children = children;
            }

            public string Label { get; private set; }
            public IEnumerable<TreeNode> Children { get; private set; }
        }
    }
}
