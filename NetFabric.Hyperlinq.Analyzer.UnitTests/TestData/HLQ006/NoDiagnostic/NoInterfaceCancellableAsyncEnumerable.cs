using System;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ006.NoDiagnostic
{
    readonly struct NoInterfaceCancellableAsyncEnumerable<T>
    {
        public AsyncEnumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator();

        public struct AsyncEnumerator
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }
    }
}
