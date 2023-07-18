using System;

namespace HLQ013.Diagnostic.Array
{
    partial class C
    {
        void Method()
        {
            var source = new[] { 1, 2, 3 };
            for (var index = 0; index < source.Length; index++)
            {
                var item = source[index];
                Console.WriteLine(item);
            }
        }
    }
}
