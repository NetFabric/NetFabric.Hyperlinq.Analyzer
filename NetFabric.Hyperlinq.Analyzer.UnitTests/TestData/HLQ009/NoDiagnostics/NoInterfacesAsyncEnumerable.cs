using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Threading.Tasks;

namespace HLQ009.NoDiagnostics
{
    readonly struct NoInterfacesAsyncEnumerable<T>
    {
        public Enumerator GetAsyncEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }
    }

    partial class C
    {
        public async ValueTask Test_NoInterfacesAsyncEnumerable()
        {
            await foreach (var _ in new NoInterfacesAsyncEnumerable<TestType>())
            {

            }
        }
    }
}
