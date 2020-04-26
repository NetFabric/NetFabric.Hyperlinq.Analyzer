using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.SimpleAssignment
{ 
    class FieldDeclarationAsync
    {
        IAsyncEnumerable<TestType> field;

        void Method()
        {
            field = new OptimizedAsyncEnumerable<TestType>();
        }
    }
}
