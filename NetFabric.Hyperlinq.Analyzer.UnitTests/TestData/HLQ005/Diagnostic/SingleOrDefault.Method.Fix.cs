using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;

namespace HLQ005.Diagnostic
{
    partial class Class
    {
        public TestType Test_SingleOrDefault_Method()
            => EnumerableExtensions.FirstOrDefault(new OptimizedEnumerable<TestType>());
    }
}
