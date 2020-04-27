using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests.TestData
{
    public readonly struct OptimizedEnumerable<T> : IEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new DisposableEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator();

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

    public class NonOptimizedEnumerable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => new Enumerator();
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

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
