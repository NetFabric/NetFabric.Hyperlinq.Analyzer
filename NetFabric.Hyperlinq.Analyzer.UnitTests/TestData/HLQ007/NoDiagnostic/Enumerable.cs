using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;

namespace HLQ007.NoDiagnostic
{
    readonly struct Enumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;
        }
    }

    class C
    {
        void Test()
        {
            // make sure implementation is supported by foreach
            foreach (var item in new Enumerable<TestType>())
                Console.WriteLine(item);
        }
    }
}
