using System;

namespace HLQ010.NoDiagnostic.ReadOnlySpan
{
    partial class C
    {
        void Method()
        {
            var source = (ReadOnlySpan<int>)new int[0].AsSpan();
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
