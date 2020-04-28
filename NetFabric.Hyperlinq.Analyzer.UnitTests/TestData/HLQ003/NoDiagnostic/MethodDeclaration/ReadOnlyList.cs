using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.ReadOnlyList
{
    public partial class C
    {
        public IReadOnlyList<TestType> MethodDeclaration()
        {
            return new OptimizedReadOnlyList<TestType>();
        }
    }
}
