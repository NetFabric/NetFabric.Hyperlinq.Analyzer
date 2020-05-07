using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ007.Diagnostic.Arrow
{
    readonly struct AsyncEnumerable<T>: IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();

        class Enumerator : IAsyncEnumerator<T>
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

            public void Reset() { }

            public ValueTask DisposeAsync() 
                =>    default;
        }
    }

    partial class Tests
    {
        async ValueTask Test_AsyncEnumerableArrow()
        {
            // make sure implementation is supported by foreach
            await foreach (var item in new AsyncEnumerable<TestType>())
            Console.WriteLine(item);
        }
    }
}
