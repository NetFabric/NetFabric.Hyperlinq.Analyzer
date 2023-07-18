using System;

namespace HLQ013.NoDiagnostic.Array
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

            for (var index = 0; index < source.Length; index++)
            {
                var item = source[index]; // using variable for indexing
                Console.WriteLine(item);
                Console.WriteLine(index); // using variable for something else
            }

            var source2 = source;
            for (var index = 0; index < source.Length; index++)
            {
                var item = source[index]; // using variable for indexing
                var item2 = source2[index]; // using variable for indexing another collection
                Console.WriteLine(item);
                Console.WriteLine(item2);
            }
        }
    }
}
