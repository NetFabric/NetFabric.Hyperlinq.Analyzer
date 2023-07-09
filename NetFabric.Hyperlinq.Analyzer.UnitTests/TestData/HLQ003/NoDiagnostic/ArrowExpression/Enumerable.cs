using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.Enumerable
{
    public partial class C
    {
        public IEnumerable<TestType> MethodArrow() 
            => new OptimizedEnumerable<TestType>();

        public OptimizedEnumerable<TestType> MethodArrow2()
            => new OptimizedEnumerable<TestType>();
    }
}
