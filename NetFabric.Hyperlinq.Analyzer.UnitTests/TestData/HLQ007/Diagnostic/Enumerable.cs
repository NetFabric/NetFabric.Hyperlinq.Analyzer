using System;

namespace HLQ007.Diagnostic
{
    readonly struct Enumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator : IDisposable
        {
            public T Current => default;

            public bool MoveNext() => false;

            public void Dispose() { }
        }
    }

    class C
    {
        void Test()
        {
            // make sure implementation is supported by foreach
            foreach (var item in new Enumerable<int>())
                Console.WriteLine(item);
        }
    }
}
