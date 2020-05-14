using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Diagnostics.CodeAnalysis;

namespace HLQ011.Diagnostic.Explicit
{
    class Enumerator<T>
    {
        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
        readonly OptimizedEnumerable<T>.Enumerator source;

        public Enumerator(OptimizedEnumerable<T>.Enumerator source)
            => this.source = source;
    }
}
