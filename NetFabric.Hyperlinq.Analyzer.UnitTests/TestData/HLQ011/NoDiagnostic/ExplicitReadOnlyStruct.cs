using System.Diagnostics.CodeAnalysis;

namespace HLQ011.NoDiagnostic.ExplicitReadOnlyStruct
{
    class Enumerator<T>
    {
        readonly ReadOnlyEnumerator<T> source;

        public Enumerator(ReadOnlyEnumerator<T> source)
            => this.source = source;
    }

    readonly struct ReadOnlyEnumerator<T>
    {
        public T Current => default;
        public bool MoveNext() => false;
    }
}
