using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HLQ002.Diagnostic.MethodDeclaration
{
    public class TestClass_Async
    {
        public IAsyncEnumerable<TestType> Method()
        {
            return null;
        }
    }
}
