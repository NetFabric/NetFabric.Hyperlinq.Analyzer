using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;

namespace HLQ009.Diagnostics
{
    readonly struct NoInterfacesEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;

            public void Reset() => throw new NotImplementedException();

            public void Dispose() { }
        }
    }

    class C
    {
        public void Test()
        {
            foreach (var _ in new NoInterfacesEnumerable<TestType>())
            {

            }
        }
    }
}
