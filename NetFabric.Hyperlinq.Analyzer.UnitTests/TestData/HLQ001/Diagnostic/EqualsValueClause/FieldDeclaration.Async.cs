using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.EqualsValueClause
{ 
    class FieldDeclarationAsync
    {
        IAsyncEnumerable<TestType> field = new OptimizedAsyncEnumerable<TestType>();
    }
}
