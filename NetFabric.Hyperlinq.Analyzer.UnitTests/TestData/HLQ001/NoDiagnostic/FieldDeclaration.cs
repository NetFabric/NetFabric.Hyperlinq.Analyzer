using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

#pragma warning disable IDE0052 // Remove unread private members

namespace HLQ001.NoDiagnostic
{
    public class FieldDeclaration
    {
        OptimizedEnumerable<TestType> field00 = new OptimizedEnumerable<TestType>();
        IEnumerable<TestType> field01 = new NonOptimizedEnumerable<TestType>();

        public IEnumerable<TestType> field11 = new OptimizedEnumerable<TestType>(); // boxes enumerator but public

        public void Method()
        {
            field00 = new OptimizedEnumerable<TestType>();
            field01 = new NonOptimizedEnumerable<TestType>();

            field11 = new OptimizedEnumerable<TestType>(); // boxes enumerator but public
        }
    }
}

#pragma warning restore IDE0052 // Remove unread private members
