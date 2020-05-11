using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.NoDiagnostic
{
    public class PropertyDeclarationAsync
    {
        OptimizedAsyncEnumerable<TestType> Property00 { get; set; } = new OptimizedAsyncEnumerable<TestType>();
        IAsyncEnumerable<TestType> Property01 { get; set; } = new NonOptimizedAsyncEnumerable<TestType>();

        public IAsyncEnumerable<TestType> Property11 { get; set; } = new OptimizedAsyncEnumerable<TestType>(); // boxes enumerator but public

        public void Method()
        {
            Property00 = new OptimizedAsyncEnumerable<TestType>();
            Property01 = new NonOptimizedAsyncEnumerable<TestType>();

            Property11 = new OptimizedAsyncEnumerable<TestType>(); // boxes enumerator but public
        }
    }
}
