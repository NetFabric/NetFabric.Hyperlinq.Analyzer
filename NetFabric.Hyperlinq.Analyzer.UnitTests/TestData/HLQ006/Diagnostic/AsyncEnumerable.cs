using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ006.Diagnostic
{
    class AsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) => new Enumerator();

        public class Enumerator : IAsyncEnumerator<T>
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

            public ValueTask DisposeAsync() => default;
        }
    }
}
