using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests.TestData
{
    public readonly struct OptimizedReadOnlyList<T> : IReadOnlyList<T>, IList<T>
    {
        public int Count => 0;

        public T this[int index] 
        { 
            get => throw new IndexOutOfRangeException(); 
            set => throw new IndexOutOfRangeException(); 
        }

        bool ICollection<T>.IsReadOnly => true;

        public Enumerator GetEnumerator() => new Enumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new DisposableEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator();

        public bool Contains(T item) => false;
        public void CopyTo(T[] array, int arrayIndex) { }
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();

        public int IndexOf(T item) => -1;
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;
        }

        class DisposableEnumerator : IEnumerator<T>
        {
            public T Current => default;
            object IEnumerator.Current => default;

            public bool MoveNext() => false;

            public void Reset() { }

            public void Dispose() { }
        }
    }

    public class NonOptimizedReadOnlyList<T> : IReadOnlyList<T>, IList<T>
    {
        public int Count => 0;

        public T this[int index]
        {
            get => throw new IndexOutOfRangeException();
            set => throw new IndexOutOfRangeException();
        }

        bool ICollection<T>.IsReadOnly => true;

        public IEnumerator<T> GetEnumerator() => new Enumerator();
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

        public bool Contains(T item) => false;
        public void CopyTo(T[] array, int arrayIndex) { }
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();

        public int IndexOf(T item) => -1;
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();

        class Enumerator : IEnumerator<T>
        {
            public T Current => default;
            object IEnumerator.Current => default;

            public bool MoveNext() => false;

            public void Reset() { }

            public void Dispose() { }
        }
    }
}
