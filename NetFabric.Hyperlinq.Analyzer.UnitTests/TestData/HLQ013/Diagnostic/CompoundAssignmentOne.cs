using System;

namespace HLQ013.Diagnostic.CompoundAssignmentOneTest
{
    partial class C
    {
        void Method()
        {
            var source = new[] { 1, 2, 3 };
            for (var index = 0; index < source.Length; index += 1)       
            {
                var item = source[index];
                Console.WriteLine(item);
            }
        }
    }
}
