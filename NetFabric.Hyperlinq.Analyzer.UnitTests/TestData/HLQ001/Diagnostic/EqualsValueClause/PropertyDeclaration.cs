using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.EqualsValueClause
{
    class PropertyDeclaration
    {
        IEnumerable<TestType> Property { get; set; } = new OptimizedEnumerable<TestType>();
    }
}
