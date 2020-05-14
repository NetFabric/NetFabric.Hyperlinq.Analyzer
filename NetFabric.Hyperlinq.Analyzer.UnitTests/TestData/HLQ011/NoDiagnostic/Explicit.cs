using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System.Diagnostics.CodeAnalysis;

namespace HLQ011.NoDiagnostic.Explicit
{
    class Enumerator<T>
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier")]
        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
        OptimizedEnumerable<T>.Enumerator source;

        public Enumerator(OptimizedEnumerable<T>.Enumerator source)
            => this.source = source;
    }
}
