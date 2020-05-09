using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Threading.Tasks;

namespace HLQ009.Diagnostics.Async
{
    readonly struct AsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }
    }

    class C
    {
        public async ValueTask TestAsync()
        {
            await foreach (var _ in new AsyncEnumerable<TestType>())
            {

            }
        }
    }
}
