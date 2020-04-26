using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.EqualsValueClause
{
    class VariableDeclarationAsync
    {
        public void Method()
        {
            IAsyncEnumerable<TestType> variable = new OptimizedAsyncEnumerable<TestType>();
        }
    }
}
