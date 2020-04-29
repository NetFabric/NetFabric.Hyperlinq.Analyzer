using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ003.NoDiagnostic.ReadOnlyList.ReadOnlyCollection
{
    public partial class C
    {
        public IReadOnlyCollection<TestType> MethodDeclaration(int i)
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
