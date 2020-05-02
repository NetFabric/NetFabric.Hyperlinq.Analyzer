using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;

namespace HLQ004.NoDiagnostic
{
    class NoRef
    {
        void Method()
        {
            foreach (var item in new OptimizedEnumerable<TestType>())
            {

            }
        }
    }
}
