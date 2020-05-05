using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;

namespace HLQ005.Diagnostic
{
    partial class Class
    {
        public TestType Test_SingleOrDefault_Extensions()
            => new OptimizedEnumerable<TestType>().SingleOrDefault();
    }
}
