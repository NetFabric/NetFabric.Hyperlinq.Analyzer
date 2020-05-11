using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

#pragma warning disable IDE0052 // Remove unread private members

namespace HLQ001.NoDiagnostic
{
    public class FieldDeclarationAsync
    {
        OptimizedAsyncEnumerable<TestType> field00 = new OptimizedAsyncEnumerable<TestType>();
        IAsyncEnumerable<TestType> field01 = new NonOptimizedAsyncEnumerable<TestType>();

        public IAsyncEnumerable<TestType> field11 = new OptimizedAsyncEnumerable<TestType>(); // boxes enumerator but public

        public void Method()
        {
            field00 = new OptimizedAsyncEnumerable<TestType>();
            field01 = new NonOptimizedAsyncEnumerable<TestType>();

            field11 = new OptimizedAsyncEnumerable<TestType>(); // boxes enumerator but public
        }
    }
}

#pragma warning restore IDE0052 // Remove unread private members
