using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ003.Diagnostic.ReadOnlyList.Enumerable
{
    public partial class C
    {
        public IEnumerable<TestType> MethodArrow() 
            => new OptimizedReadOnlyList<TestType>();
    }
}
