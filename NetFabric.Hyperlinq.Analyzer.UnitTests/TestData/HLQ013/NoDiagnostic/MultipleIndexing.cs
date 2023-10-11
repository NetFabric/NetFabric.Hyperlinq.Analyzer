using System;

namespace HLQ013.NoDiagnostic.MultipleIndexingTest
{
    partial class C
    {
        void Method()
        {
            var source = new[] { 1, 2, 3 };

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
