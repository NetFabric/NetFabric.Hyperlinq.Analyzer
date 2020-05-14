using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HLQ011.Diagnostic.Generic
{
    class Enumerator<TEnumerator> where TEnumerator : struct, IEnumerator
    {
        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
        readonly TEnumerator source;

        public Enumerator(TEnumerator source)
            => this.source = source;
    }
}
