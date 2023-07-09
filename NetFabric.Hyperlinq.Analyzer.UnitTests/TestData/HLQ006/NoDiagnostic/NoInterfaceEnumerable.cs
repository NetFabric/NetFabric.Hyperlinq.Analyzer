namespace HLQ006.NoDiagnostic
{
    readonly struct NoInterfaceEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;
        }
    }
}
