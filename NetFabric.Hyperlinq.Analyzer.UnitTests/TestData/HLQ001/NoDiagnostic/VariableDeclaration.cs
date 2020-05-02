using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

#pragma warning disable CS0219 // Variable is assigned but its value is never used
#pragma warning disable IDE0059 // Unnecessary assignment of a value

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

#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0219 // Variable is assigned but its value is never used

