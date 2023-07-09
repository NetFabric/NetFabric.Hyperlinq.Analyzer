namespace HLQ008.NoDiagnostic
{
    readonly ref struct RefValueTypeEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;
        }
    }
}
