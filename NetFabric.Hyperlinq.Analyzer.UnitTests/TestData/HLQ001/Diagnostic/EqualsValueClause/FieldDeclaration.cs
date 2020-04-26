using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.EqualsValueClause
{ 
    class FieldDeclaration
    {
        IEnumerable<TestType> field = new OptimizedEnumerable<TestType>();
    }
}
