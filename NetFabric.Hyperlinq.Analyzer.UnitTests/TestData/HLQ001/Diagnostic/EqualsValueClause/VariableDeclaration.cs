using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.EqualsValueClause
{
    class VariableDeclaration
    {
        public void Method()
        {
            IEnumerable<TestType> variable = new OptimizedEnumerable<TestType>();
        }
    }
}
