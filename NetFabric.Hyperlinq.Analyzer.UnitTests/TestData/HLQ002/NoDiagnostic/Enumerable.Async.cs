using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HLQ002.NoDiagnostic
{
    public class TestClass_Async
    {
        public async IAsyncEnumerable<TestType> Method_AsyncIterator()
        {
             yield return await Task.FromResult(new TestType());
        }

        public IAsyncEnumerable<TestType> Method()
        {
            return Method_AsyncIterator();
        }

        public IAsyncEnumerable<TestType> MethodArrow() 
            => Method_AsyncIterator();
    }
}
