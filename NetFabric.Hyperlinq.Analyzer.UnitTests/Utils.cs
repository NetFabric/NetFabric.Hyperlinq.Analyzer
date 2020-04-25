using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public static class Utils
    {
        public static CSharpCompilation Compile(params string[] paths)
            => CSharpCompilation.Create(
                Guid.NewGuid().ToString(),
                paths.Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path))),
                GetMetadataReferences(
                    typeof(IEnumerable),
                    typeof(IAsyncEnumerable<>),
                    typeof(Enumerable),
                    typeof(AsyncEnumerable),
                    typeof(ValueTask)),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        public static ITypeSymbol GetTypeSymbol(this CSharpCompilation compilation, Type type)
        {
            if (type == typeof(int))
                return compilation.GetSpecialType(SpecialType.System_Int32);

            var symbol = compilation.GetTypeByMetadataName(type.FullName);

            if (type.IsGenericType)
            {
                var typeArguments = type.GenericTypeArguments
                    .Select(argumentType => compilation.GetTypeSymbol(argumentType));
                return symbol.Construct(typeArguments.ToArray());
            }

            return symbol;
        }

        static IEnumerable<MetadataReference> GetMetadataReferences(params Type[] types)
            => types.Select(type => MetadataReference.CreateFromFile(type.Assembly.Location));
    }
}
