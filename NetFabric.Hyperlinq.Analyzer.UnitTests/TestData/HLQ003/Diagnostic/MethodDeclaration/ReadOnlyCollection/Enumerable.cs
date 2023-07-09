using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ003.Diagnostic.ReadOnlyCollection.Enumerable
{
    public partial class C
    {
        public IEnumerable<TestType> MethodDeclaration(int i)
        {
            switch (i)
            {
                case 1:
                    return new OptimizedReadOnlyList<TestType>();

                case 0:
                    return new OptimizedReadOnlyCollection<TestType>();

                default:
                    return new OptimizedReadOnlyCollection<TestType>();
            }
        }
    }
}
