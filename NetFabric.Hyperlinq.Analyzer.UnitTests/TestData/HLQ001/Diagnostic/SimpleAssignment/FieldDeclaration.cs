using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.SimpleAssignment
{ 
    class FieldDeclaration
    {
        IEnumerable<TestType> field;

        void Method()
        {
            field = new OptimizedEnumerable<TestType>();
        }
    }
}
