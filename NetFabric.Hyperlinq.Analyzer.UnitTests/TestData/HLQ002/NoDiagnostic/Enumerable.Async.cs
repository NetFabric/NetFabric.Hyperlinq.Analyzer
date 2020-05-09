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

#pragma warning disable IDE0022 // Use expression body for methods
        public IAsyncEnumerable<TestType> Method()
        {
            return Method_AsyncIterator();
        }
#pragma warning restore IDE0022 // Use expression body for methods

        public IAsyncEnumerable<TestType> MethodArrow() 
            => Method_AsyncIterator();
    }
}
