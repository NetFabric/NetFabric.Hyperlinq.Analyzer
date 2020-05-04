using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;

namespace HLQ005.Diagnostic
{
    partial class Class
    {
        public TestType Test_Single_Extensions()
            => new OptimizedEnumerable<TestType>().First();
    }
}
