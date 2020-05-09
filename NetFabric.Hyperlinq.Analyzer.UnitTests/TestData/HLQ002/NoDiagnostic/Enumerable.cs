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

#pragma warning disable IDE0022 // Use expression body for methods
        public IEnumerable<TestType> Method()
        {
            return Method_Iterator();
        }
#pragma warning restore IDE0022 // Use expression body for methods

        public IEnumerable<TestType> MethodArrow() 
            => Method_Iterator();

#pragma warning disable IDE0022 // Use expression body for methods
        public string StringMethod()
        {
            return null;
        }
#pragma warning restore IDE0022 // Use expression body for methods

        public string StringMethodArrow()
            => null;
    }
}
