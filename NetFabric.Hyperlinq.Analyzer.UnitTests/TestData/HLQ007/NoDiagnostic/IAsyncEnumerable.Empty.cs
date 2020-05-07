using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ007.NoDiagnostic.Empty
{
    readonly struct AsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) => new DisposableEnumerator();

        public struct Enumerator
        {
            public int Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }

        class DisposableEnumerator : IAsyncEnumerator<T>
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

            public void Reset() { }

            public ValueTask DisposeAsync() => default;
        }
    }

    partial class Tests
    {
        async ValueTask Test_AsyncEnumerable()
        {
            // make sure implementation is supported by foreach
            await foreach (var item in new AsyncEnumerable<TestType>())
                Console.WriteLine(item);
        }
    }
}
