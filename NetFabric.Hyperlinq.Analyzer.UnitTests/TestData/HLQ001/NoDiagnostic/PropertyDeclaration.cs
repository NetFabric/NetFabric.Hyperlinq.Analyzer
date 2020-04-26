using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.NoDiagnostic
{
    class PropertyDeclaration
    {
        OptimizedEnumerable<TestType> Property00 { get; set; } = new OptimizedEnumerable<TestType>();
        IEnumerable<TestType> Property01 { get; set; } = new NonOptimizedEnumerable<TestType>();

        public IEnumerable<TestType> Property11 { get; set; } = new OptimizedEnumerable<TestType>(); // boxes enumerator but public

        public void Method()
        {
            Property00 = new OptimizedEnumerable<TestType>();
            Property01 = new NonOptimizedEnumerable<TestType>();

            Property11 = new OptimizedEnumerable<TestType>(); // boxes enumerator but public
        }
    }
}
