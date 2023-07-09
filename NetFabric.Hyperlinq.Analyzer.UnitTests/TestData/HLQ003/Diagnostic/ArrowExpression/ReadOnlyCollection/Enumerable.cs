using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ003.Diagnostic.ReadOnlyCollection.Enumerable
{
    public partial class C
    {
        public IEnumerable<TestType> MethodArrow() 
            => new OptimizedReadOnlyCollection<TestType>();
    }
}
