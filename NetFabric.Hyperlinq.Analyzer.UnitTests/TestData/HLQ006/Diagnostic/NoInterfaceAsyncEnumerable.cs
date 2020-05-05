using System;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ006.Diagnostic
{
    class NoInterfaceAsyncEnumerable<T>
    {
        public AsyncEnumerator GetAsyncEnumerator() => new AsyncEnumerator();

        public class AsyncEnumerator
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }
    }
}
