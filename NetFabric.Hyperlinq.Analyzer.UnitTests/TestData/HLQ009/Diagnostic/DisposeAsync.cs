using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Threading.Tasks;

namespace HLQ009.Diagnostics.Async
{
    readonly struct DisposeAsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator() => new Enumerator();

        public struct Enumerator : IAsyncDisposable
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

            public ValueTask DisposeAsync() => new ValueTask();
        }
    }

    partial class C
    {
        public async ValueTask Test_DisposeAsyncEnumerable()
        {
            await foreach (var _ in new DisposeAsyncEnumerable<TestType>())
            {

            }
        }
    }
}
