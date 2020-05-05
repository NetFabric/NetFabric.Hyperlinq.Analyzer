using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HLQ005.NoDiagnostic
{
    class AsyncClass
    {
        public ValueTask<TestType> SingleAsync(TestType source)
            => default;

        public ValueTask<TestType> SingleOrDefaultAsync(TestType source)
            => default;

        public void Method()
        {
            var a0 = new OptimizedAsyncEnumerable<TestType>().FirstAsync();
            var b0 = new OptimizedAsyncEnumerable<TestType>().FirstOrDefaultAsync();
            var c0 = AsyncEnumerableExtensions.FirstAsync(new OptimizedAsyncEnumerable<TestType>());
            var d0 = AsyncEnumerableExtensions.FirstOrDefaultAsync(new OptimizedAsyncEnumerable<TestType>());

            var a1 = new TestType().SingleAsync();
            var b1 = new TestType().SingleOrDefaultAsync();
            var c1 = NotAsyncEnumerableExtensions.SingleAsync(new TestType());
            var d1 = NotAsyncEnumerableExtensions.SingleOrDefaultAsync(new TestType());

            var c2 = SingleAsync(new TestType());
            var d2 = SingleOrDefaultAsync(new TestType());
        }
    }

    static class NotAsyncEnumerableExtensions
    {
        public static ValueTask<TestType> SingleAsync(this TestType source)
            => default;

        public static ValueTask<TestType> SingleOrDefaultAsync(this TestType source)
            => default;
    }
}
