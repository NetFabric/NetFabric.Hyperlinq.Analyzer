using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.ReadOnlyList
{
    public partial class C
    {
        public IReadOnlyList<TestType> MethodArrow() 
            => new OptimizedReadOnlyList<TestType>();
    }
}
