#pragma warning disable IDE0022 // Use expression body for methods

using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ007.Diagnostic.Block
{
    readonly struct AsyncEnumerable<T>: IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();

        class Enumerator: IAsyncEnumerator<T>
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

            public void Reset() { }

            public ValueTask DisposeAsync()
            {
                return default;
            }
        }
    }

    partial class Tests
    {
        async ValueTask Test_AsyncEnumerableBlock()
        {
            // make sure implementation is supported by foreach
            await foreach (var item in new AsyncEnumerable<TestType>())
                Console.WriteLine(item);
        }
    }
}

#pragma warning restore IDE0022 // Use expression body for methods
