using System;

namespace HLQ013.NoDiagnostic.CompoundAssignmentNotOneTest
{
    partial class C
    {
        void Method()
        {
            var source = new[] { 1, 2, 3 };

            for (var index = 0; index < source.Length; index += 2)  // not incrementing by 1
            {
                var item = source[index];
                Console.WriteLine(item);
            }
        }
    }
}
