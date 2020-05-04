using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Threading.Tasks;

namespace HLQ005.Diagnostic
{
    partial class Class
    {
        public ValueTask<TestType> Test_SingleAsync_Method()
            => AsyncEnumerableExtensions.SingleAsync(new OptimizedAsyncEnumerable<TestType>());
    }
}
