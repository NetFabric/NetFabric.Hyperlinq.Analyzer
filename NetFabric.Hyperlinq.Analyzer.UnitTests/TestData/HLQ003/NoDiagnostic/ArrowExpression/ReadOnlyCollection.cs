using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.ReadOnlyCollection
{
    public partial class C
    {
        public IReadOnlyCollection<TestType> MethodArrow() 
            => new OptimizedReadOnlyCollection<TestType>();
    }
}
