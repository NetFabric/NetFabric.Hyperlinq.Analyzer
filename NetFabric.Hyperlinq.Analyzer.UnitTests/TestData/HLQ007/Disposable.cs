using System;
using System.Threading.Tasks;

namespace HLQ007
{
    class Disposable : IDisposable
    {
        public void Dispose() { }
    }

    class AsyncDisposable: IAsyncDisposable
    {
        public ValueTask DisposeAsync() => default;
    }
}
