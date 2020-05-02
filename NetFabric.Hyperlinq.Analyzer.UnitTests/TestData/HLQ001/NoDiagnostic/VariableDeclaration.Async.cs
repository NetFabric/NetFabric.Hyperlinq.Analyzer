using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable CS0219 // Variable is assigned but its value is never used
#pragma warning disable IDE0059 // Unnecessary assignment of a value

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

#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0219 // Variable is assigned but its value is never used
