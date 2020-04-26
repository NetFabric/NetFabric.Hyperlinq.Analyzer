using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections.Generic;

namespace HLQ002.NoDiagnostic
{
    public partial class TestClass
    {
        public IEnumerable<TestType> Method_Iterator()
        {
            yield return new TestType();
        }

        public IEnumerable<TestType> Method()
        {
            return Method_Iterator();
        }

        public IEnumerable<TestType> MethodArrow() 
            => Method_Iterator();
    }
}
