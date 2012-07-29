using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal class AutoCastingList<T, U> : IList<T> where T : U
    {
        private readonly IList<U> innerList;

        public AutoCastingList(IList<U> wrappedList)
        {
            this.innerList = wrappedList ?? new List<U>();
        }

        public int IndexOf(T item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return (T)innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        public void Add(T item)
        {
            innerList.Add(item);
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(T item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.GetEnumerable().ToArray().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return innerList.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return innerList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return innerList.Cast<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<T> GetEnumerable()
        {
            return innerList.Cast<T>();
        }
    }
}
