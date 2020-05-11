using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;

namespace HLQ009.NoDiagnostics
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

    partial class C
    {
        public void Test_Enumerable()
        {
            foreach (var _ in new Enumerable<TestType>())
            {

            }
        }
    }
}
