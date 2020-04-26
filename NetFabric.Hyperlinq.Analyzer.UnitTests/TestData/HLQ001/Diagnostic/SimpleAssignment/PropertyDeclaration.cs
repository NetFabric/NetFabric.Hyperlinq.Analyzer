using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Collections.Generic;

namespace HLQ001.Diagnostic.SimpleAssignment
{
    class PropertyDeclaration
    {
        IEnumerable<TestType> Property { get; set; }

        void Method()
        {
            Property = new OptimizedEnumerable<TestType>();
        }
    }
}
