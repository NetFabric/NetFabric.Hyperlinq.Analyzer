using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ005.NoDiagnostic
{
    class Class
    {
        public TestType Single(TestType source)
            => default;

        public TestType SingleOrDefault(TestType source)
            => default;

        public void Method()
        {
            var a0 = new OptimizedEnumerable<TestType>().First();
            var b0 = new OptimizedEnumerable<TestType>().FirstOrDefault();
            var c0 = EnumerableExtensions.First(new OptimizedEnumerable<TestType>());
            var d0 = EnumerableExtensions.FirstOrDefault(new OptimizedEnumerable<TestType>());

            var a1 = new TestType().Single();
            var b1 = new TestType().SingleOrDefault();
            var c1 = NotEnumerableExtensions.Single(new TestType());
            var d1 = NotEnumerableExtensions.SingleOrDefault(new TestType());

            var a2 = Single(new TestType());
            var b2 = SingleOrDefault(new TestType());
        }
    }

    static class NotEnumerableExtensions
    {
        public static T Single<T>(this T source)
            => default;

        public static T Single<T>(this T source, Func<T, bool> predicate)
            => default;

        public static T SingleOrDefault<T>(this T source)
            => default;

        public static T SingleOrDefault<T>(this T source, Func<TestType, bool> predicate)
            => default;
    }
}
