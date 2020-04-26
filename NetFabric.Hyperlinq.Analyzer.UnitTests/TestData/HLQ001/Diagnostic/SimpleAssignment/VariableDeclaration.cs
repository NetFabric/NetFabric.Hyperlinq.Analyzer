using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.SimpleAssignment
{
    class VariableDeclaration
    {
        public void Method()
        {
            IEnumerable<TestType> variable;
            
            variable = new OptimizedEnumerable<TestType>();
        }
    }
}
