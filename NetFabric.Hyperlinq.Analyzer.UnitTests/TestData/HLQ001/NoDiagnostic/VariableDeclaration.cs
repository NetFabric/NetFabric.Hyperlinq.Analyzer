using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.NoDiagnostic
{
    class VariableDeclaration
    {
        public void Method()
        {
            OptimizedEnumerable<TestType> variable00 = new OptimizedEnumerable<TestType>();
            IEnumerable<TestType> variable01 = new NonOptimizedEnumerable<TestType>();

            var variable10 = new OptimizedEnumerable<TestType>();

            variable00 = new OptimizedEnumerable<TestType>();
            variable01 = new NonOptimizedEnumerable<TestType>();

            variable10 = new OptimizedEnumerable<TestType>();
        }
    }
}
