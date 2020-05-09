using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;

namespace HLQ009.NoDiagnostics
{
    readonly struct NoInterfacesEnumerable<T>
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
        public void Test_NoInterfacesEnumerable()
        {
            foreach (var _ in new NoInterfacesEnumerable<TestType>())
            {

            }
        }
    }
}
