using Microsoft.CodeAnalysis;

namespace NetFabric.Hyperlinq.Analyzer;

static class TypeSymbolExtensions
{
    public static bool IsArrayType(this ITypeSymbol typeSymbol) 
        => typeSymbol is IArrayTypeSymbol;

    public static bool IsSpanType(this ITypeSymbol typeSymbol) 
        => typeSymbol.ToString().StartsWith("System.Span<") || typeSymbol.ToString().StartsWith("System.ReadOnlySpan<");
}
