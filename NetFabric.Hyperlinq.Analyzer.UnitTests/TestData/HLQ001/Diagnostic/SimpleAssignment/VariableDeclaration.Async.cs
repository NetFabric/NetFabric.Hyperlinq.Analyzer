using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.SimpleAssignment
{
    class VariableDeclarationAsync
    {
        public void Method()
        {
            IAsyncEnumerable<TestType> variable;
            
            variable = new OptimizedAsyncEnumerable<TestType>();
        }
    }
}
