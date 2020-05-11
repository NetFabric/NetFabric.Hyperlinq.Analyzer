using HLQ007;
using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ009.NoDiagnostics
{
    readonly struct DisposeAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator() => new Enumerator();
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) => new Enumerator();

        public struct Enumerator : IAsyncEnumerator<T>
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
