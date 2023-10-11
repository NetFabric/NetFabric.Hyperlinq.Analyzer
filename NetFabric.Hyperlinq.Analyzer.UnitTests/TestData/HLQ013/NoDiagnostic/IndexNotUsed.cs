using System;

namespace HLQ013.NoDiagnostic.IndexNotUsedTest
{
    partial class C
    {
        void Method()
        {
            var source = new[] { 1, 2, 3 };

            for (var index = 0; index < source.Length; index++)
            {
                // variable not used for indexing
                Console.WriteLine(index);
            }
        }
    }
}
