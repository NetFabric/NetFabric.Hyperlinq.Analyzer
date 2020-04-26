using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

    public readonly struct OptimizedAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new Enumerator();
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
            => new DisposableEnumerator();

        public struct Enumerator
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }

        class DisposableEnumerator : IAsyncEnumerator<T>
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

            public ValueTask DisposeAsync() => new ValueTask();
        }
    }

    public class NonOptimizedAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new Enumerator();

        class Enumerator : IAsyncEnumerator<T>
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

            public ValueTask DisposeAsync() => new ValueTask();
        }
    }

}
