using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ003.Diagnostic.ReadOnlyCollection.Enumerable
{
    public partial class C
    {
        public IEnumerable<TestType> MethodDeclaration()
        {
            return new OptimizedReadOnlyCollection<TestType>();
        }
    }
}
