using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.YieldReturn
{
    public partial class C
    {
        public IEnumerable<TestType> MethodDeclaration()
        {
            yield return new TestType();
        }
    }
}
