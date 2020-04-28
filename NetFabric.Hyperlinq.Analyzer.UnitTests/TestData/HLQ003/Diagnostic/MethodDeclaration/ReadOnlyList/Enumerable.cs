using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.ReadOnlyList.Enumerable
{
    public partial class C
    {
        public IEnumerable<TestType> MethodDeclaration()
        {
            return new OptimizedReadOnlyList<TestType>();
        }
    }
}
