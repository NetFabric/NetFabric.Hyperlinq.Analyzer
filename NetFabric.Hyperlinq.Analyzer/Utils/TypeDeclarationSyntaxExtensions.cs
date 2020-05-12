using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class TypeDeclarationSyntaxExtensions
    {
        const char NESTED_CLASS_DELIMITER = '+';
        const char NAMESPACE_CLASS_DELIMITER = '.';
        const char TYPEPARAMETER_CLASS_DELIMITER = '`';

        public static bool IsValueType(this TypeDeclarationSyntax typeDeclarationSyntax)
            => typeDeclarationSyntax is StructDeclarationSyntax;

        public static bool IsEnumerable(this TypeDeclarationSyntax typeDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            var name = typeDeclarationSyntax.GetMetadataName();
            var declaredTypeSymbol = context.Compilation.GetTypeByMetadataName(name);
            return declaredTypeSymbol is object 
                && (declaredTypeSymbol.IsEnumerable(context.Compilation, out var _) 
                || declaredTypeSymbol.IsAsyncEnumerable(context.Compilation, out var _));
        }

        public static bool ImplementsInterface(this TypeDeclarationSyntax typeDeclarationSyntax, SpecialType interfaceType, SyntaxNodeAnalysisContext context)
        {
            var baseList = typeDeclarationSyntax.BaseList;
            if (baseList is object)
            {
                foreach (var type in baseList.Types.Select(baseType => baseType.Type))
                {
                    var typeSymbol = context.SemanticModel.GetTypeInfo(type).Type;
                    if (typeSymbol is object 
                        && (typeSymbol.OriginalDefinition.SpecialType == interfaceType 
                        || typeSymbol.ImplementsInterface(interfaceType, out var _)))
                        return true;
                }
            }

            return false;
        }

        public static bool ImplementsInterface(this TypeDeclarationSyntax typeDeclarationSyntax, INamedTypeSymbol interfaceType, SyntaxNodeAnalysisContext context)
        {
            var baseList = typeDeclarationSyntax.BaseList;
            if (baseList is object)
            {
                foreach (var type in baseList.Types.Select(baseType => baseType.Type))
                {
                    var typeSymbol = context.SemanticModel.GetTypeInfo(type).Type;
                    if (typeSymbol is object
                        && (SymbolEqualityComparer.Default.Equals(typeSymbol.OriginalDefinition, interfaceType)
                        || typeSymbol.ImplementsInterface(interfaceType, out var _)))
                        return true;
                }
            }

            return false;
        }

        // based on https://stackoverflow.com/a/52694992
        public static string GetMetadataName(this TypeDeclarationSyntax typeDeclarationSyntax)
        {
            if (typeDeclarationSyntax is null)
                throw new ArgumentNullException(nameof(typeDeclarationSyntax));

            var namespaces = new LinkedList<NamespaceDeclarationSyntax>();
            var types = new LinkedList<TypeDeclarationSyntax>();
            for (var parent = typeDeclarationSyntax.Parent; parent is object; parent = parent.Parent)
            {
                if (parent is NamespaceDeclarationSyntax @namespace)
                {
                    _ = namespaces.AddFirst(@namespace);
                }
                else if (parent is TypeDeclarationSyntax type)
                {
                    _ = types.AddFirst(type);
                }
            }

            var result = new StringBuilder();
            for (var item = namespaces.First; item is object; item = item.Next)
            {
                _ = result.Append(item.Value.Name).Append(NAMESPACE_CLASS_DELIMITER);
            }
            for (var item = types.First; item is object; item = item.Next)
            {
                var type = item.Value;
                AppendName(result, type);
                _ = result.Append(NESTED_CLASS_DELIMITER);
            }
            AppendName(result, typeDeclarationSyntax);

            return result.ToString();
        }

        static void AppendName(StringBuilder builder, TypeDeclarationSyntax type)
        {
            _ = builder.Append(type.Identifier.Text);
            var typeArguments = type.TypeParameterList?.ChildNodes()
                .Count(node => node is TypeParameterSyntax) ?? 0;
            if (typeArguments != 0)
                _ = builder.Append(TYPEPARAMETER_CLASS_DELIMITER).Append(typeArguments);
        }
    }
}
