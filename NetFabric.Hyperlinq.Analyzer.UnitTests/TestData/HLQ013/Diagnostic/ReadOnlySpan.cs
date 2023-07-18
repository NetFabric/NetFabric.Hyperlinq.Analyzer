using System;

namespace HLQ013.Diagnostic.ReadOnlySpan
{
    partial class C
    {
        void Method()
        {
            var source = (ReadOnlySpan<int>)(new[] { 1, 2, 3 }).AsSpan();
            for (var index = 0; index < source.Length; index++)
            {
                var item = source[index];
                Console.WriteLine(item);
            }
        }
    }
}
