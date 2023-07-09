using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests.TestData
{

    public readonly struct OptimizedAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator()
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
