using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Text;

namespace HLQ001.NoDiagnostic
{
    class VariableDeclarationAsync
    {
        public void Method()
        {
            OptimizedAsyncEnumerable<TestType> variable00 = new OptimizedAsyncEnumerable<TestType>();
            IAsyncEnumerable<TestType> variable01 = new NonOptimizedAsyncEnumerable<TestType>();

            var variable10 = new OptimizedAsyncEnumerable<TestType>();

            variable00 = new OptimizedAsyncEnumerable<TestType>();
            variable01 = new NonOptimizedAsyncEnumerable<TestType>();

            variable10 = new OptimizedAsyncEnumerable<TestType>();
        }
    }
}
