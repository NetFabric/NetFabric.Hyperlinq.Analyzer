using System.Threading;
using System.Threading.Tasks;

namespace HLQ006.Diagnostic
{
    class NoInterfaceCancellableAsyncEnumerable<T>
    {
        public AsyncEnumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator();

        public class AsyncEnumerator
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }
    }
}
