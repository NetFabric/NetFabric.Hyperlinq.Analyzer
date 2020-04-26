using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.EqualsValueClause
{
    class PropertyDeclarationAsync
    {
        IAsyncEnumerable<TestType> Property { get; set; } = new OptimizedAsyncEnumerable<TestType>();
    }
}
