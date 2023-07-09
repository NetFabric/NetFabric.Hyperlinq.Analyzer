using System;

namespace HLQ010.NoDiagnostic.ReadOnlySpan
{
    partial class C
    {
        void Method()
        {
            ReadOnlySpan<int> source = new int[0].AsSpan();
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
