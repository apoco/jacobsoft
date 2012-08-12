using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static IDictionary<K, V> OrEmpty<K, V>(this IDictionary<K, V> dict)
        {
            return dict ?? new Dictionary<K, V>();
        }

        public static IEnumerable<T> Descendents<T>(
            this T root, 
            Func<T, IEnumerable<T>> nodeChildrenFunc)
        {
            return nodeChildrenFunc(root)
                .OrEmpty()
                .SelectMany(n => n.TreeNodes(nodeChildrenFunc));
        }

        public static IEnumerable<T> TreeNodes<T>(
            this T root, 
            Func<T, IEnumerable<T>> nodeChildrenFunc)
        {
            return new[] { root }.Concat(root.Descendents(nodeChildrenFunc));
        }
    }
}
