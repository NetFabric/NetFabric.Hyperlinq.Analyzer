using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;

namespace HLQ009.Diagnostics
{
    readonly struct ResetEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;

            public void Reset() => throw new NotImplementedException();
        }
    }

    partial class C
    {
        public void Test_ResetEnumerable()
        {
            foreach (var _ in new ResetEnumerable<TestType>())
            {

            }
        }
    }
}
