using System;

namespace HLQ010.NoDiagnostic.ImmutableArray
{
    partial class C
    {
        void Method()
        {
            var source = System.Collections.Immutable.ImmutableArray.Create(System.Array.Empty<int>());
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
