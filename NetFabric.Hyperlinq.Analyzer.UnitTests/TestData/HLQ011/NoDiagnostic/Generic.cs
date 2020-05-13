using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HLQ011.NoDiagnostic.Generic
{
    class Enumerator<TEnumerator> where TEnumerator : struct, IEnumerator
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier")]
        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
        TEnumerator source;

        public Enumerator(TEnumerator source)
            => this.source = source;
    }
}
