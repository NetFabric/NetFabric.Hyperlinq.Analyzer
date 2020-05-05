using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;

namespace HLQ005.Diagnostic
{
    partial class Class
    {
        public TestType Test_Single_Predicate_Method()
            => EnumerableExtensions.Single(new OptimizedEnumerable<TestType>());
    }
}
