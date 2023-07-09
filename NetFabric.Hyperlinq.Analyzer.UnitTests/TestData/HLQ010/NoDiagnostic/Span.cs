using System;

namespace HLQ010.NoDiagnostic.Span
{
    partial class C
    {
        void Method()
        {
            var source = new int[0].AsSpan();
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
