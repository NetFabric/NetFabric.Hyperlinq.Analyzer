using System;
using System.Collections.Immutable;

namespace HLQ013.NoDiagnostic.ImmutableArrayTest
{
    partial class C
    {
        void Method()
        {
            var source = ImmutableArray.Create(new[] { 1, 2, 3 });
            for (var index = 0; index < source.Length; index++)
            {
                var item = source[index];         
                Console.WriteLine(item);
            }
        }
    }
}
