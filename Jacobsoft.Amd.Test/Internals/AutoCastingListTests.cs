using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jacobsoft.Amd.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class AutoCastingListTests
    {
        [TestMethod]
        public void CopyTo()
        {
            var castedList = this.ArrangeList();

            var destArray = new int[3];
            castedList.CopyTo(destArray, 0);

            Assert.IsTrue(destArray.SequenceEqual(castedList));
        }

        [TestMethod]
        public void Contains_WhenFalse()
        {
            this.TestContains(2, expectedResult: false);
        }

        [TestMethod]
        public void IndexOf()
        {
            var list = this.ArrangeList();
            Assert.AreEqual(2, list.IndexOf(5));
            Assert.AreEqual(-1, list.IndexOf(45));
        }

        [TestMethod]
        public void Contains_WhenTrue()
        {
            this.TestContains(3, expectedResult: true);
        }

        [TestMethod]
        public void Remove()
        {
            var list = this.ArrangeList();
            list.Remove(4);
            Assert.IsTrue(list.SequenceEqual(new[] { 3, 5 }));
        }

        [TestMethod]
        public void GetEnumerator()
        {
            var list = this.ArrangeList();
            var enumerator = list.GetEnumerator();
            this.TestEnumerator(enumerator);
        }

        [TestMethod]
        public void GetEnumerator_NonGeneric()
        {
            var list = this.ArrangeList() as IEnumerable;
            var enumerator = list.GetEnumerator();
            this.TestEnumerator(enumerator);
        }

        [TestMethod]
        public void Count()
        {
            Assert.AreEqual(3, this.ArrangeList().Count);
        }

        [TestMethod]
        public void IsReadOnly_WhenTrue()
        {
            var sourceList = Array.AsReadOnly(new object[] { 3, 4, 5 });
            var list = new AutoCastingList<int, object>(sourceList);
            Assert.IsTrue(list.IsReadOnly);
        }

        [TestMethod]
        public void IsReadOnly_WhenFalse()
        {
            var sourceList = new List<object> { 3, 4, 5 };
            var list = new AutoCastingList<int, object>(sourceList);
            Assert.IsFalse(list.IsReadOnly);
        }

        [TestMethod]
        public void Add()
        {
            var list = this.ArrangeList();
            list.Add(76);
            Assert.IsTrue(list.SequenceEqual(new[] { 3, 4, 5, 76 }));
        }

        [TestMethod]
        public void Insert()
        {
            var list = this.ArrangeList();
            list.Insert(1, 76);
            Assert.IsTrue(list.SequenceEqual(new[] { 3, 76, 4, 5 }));
        }

        [TestMethod]
        public void RemoveAt()
        {
            var list = this.ArrangeList();
            list.RemoveAt(1);
            Assert.IsTrue(list.SequenceEqual(new[] { 3, 5 }));
        }

        [TestMethod]
        public void Clear()
        {
            var list = this.ArrangeList();
            list.Clear();
            Assert.IsFalse(list.Any());
        }

        [TestMethod]
        public void SetItem()
        {
            var list = this.ArrangeList();
            list[2] = 348;
            Assert.IsTrue(list.SequenceEqual(new[] { 3, 4, 348 }));
        }

        private void TestEnumerator(IEnumerator enumerator)
        {
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(4, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(5, enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }

        private void TestContains(int value, bool expectedResult)
        {
            var castedList = this.ArrangeList();
            Assert.AreEqual(expectedResult, castedList.Contains(value));
        }

        private AutoCastingList<int, object> ArrangeList()
        {
            var sourceList = new List<object> { 3, 4, 5 };
            var castedList = new AutoCastingList<int, object>(sourceList);
            return castedList;
        }
    }
}
