using Microsoft.CodeAnalysis;
using NetFabric.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.Hyperlinq.Analyzer;

static class TypeSymbolExtensions
{
    public static bool IsForEachOptimized(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol)
            return true;

        var namedTypeSymbol = typeSymbol.OriginalDefinition.ToString();
        return namedTypeSymbol 
            is "System.Span<T>" 
            or "System.ReadOnlySpan<T>" 
            or "System.Collections.Immutable.ImmutableArray<T>";
    }

    public static bool IsList(this ITypeSymbol typeSymbol, [NotNullWhen(true)] out ITypeSymbol? itemType)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol &&
            namedTypeSymbol.IsGenericType &&
            namedTypeSymbol.OriginalDefinition.ToString() == "System.Collections.Generic.List<T>")
        {
            itemType = namedTypeSymbol.TypeArguments[0];
            return true;
        }

        itemType = default;
        return false;
    }

    public static bool IsEnumerableOrAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation, [NotNullWhen(true)] out ITypeSymbol? enumeratorType)
    {
        if (typeSymbol is not null)
        {
            if (typeSymbol.IsEnumerable(compilation, out var enumerableSymbols))
            {
                enumeratorType = enumerableSymbols.GetEnumerator.ReturnType;
                return true;
            }

            if (typeSymbol.IsAsyncEnumerable(compilation, out var asyncEnumerableSymbols))
            {
                enumeratorType = asyncEnumerableSymbols.GetAsyncEnumerator.ReturnType;
                return true;
            }
        }

        enumeratorType = default;
        return false;
    }

    public static bool IsEnumerator(this ITypeSymbol type)
        => type.ImplementsInterface(SpecialType.System_Collections_IEnumerator, out _) ||
        (type.GetPublicReadProperty("Current") is not null && type.GetPublicMethod("MoveNext") is not null);

    public static bool IsAsyncEnumerator(this ITypeSymbol type, Compilation compilation)
    {
        var asyncEnumeratorType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerator`1");
        return (asyncEnumeratorType is not null && type.ImplementsInterface(asyncEnumeratorType, out _)) ||
            (type.GetPublicReadProperty("Current") is not null && type.GetPublicMethod("MoveNextAsync") is not null);
    }
}
