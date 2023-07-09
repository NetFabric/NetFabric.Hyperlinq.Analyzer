using System.Threading.Tasks;

namespace HLQ006.NoDiagnostic
{
    readonly struct NoInterfaceAsyncEnumerable<T>
    {
        public AsyncEnumerator GetAsyncEnumerator() => new AsyncEnumerator();

        public struct AsyncEnumerator
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }
    }
}
