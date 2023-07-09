using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.ReadOnlyList
{
    public partial class C
    {
        public IReadOnlyList<TestType> MethodDeclaration(int i)
        {
            switch (i)
            {
                case 1:
                    return new OptimizedReadOnlyList<TestType>();

                case 0:
                    return new OptimizedReadOnlyList<TestType>();

                default:
                    return new OptimizedReadOnlyList<TestType>();
            }
        }
    }
}
