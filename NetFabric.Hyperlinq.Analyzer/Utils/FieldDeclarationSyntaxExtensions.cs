using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class FieldDeclarationSyntaxExtensions
    {
        public static bool IsPublic(this FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            for (SyntaxNode? node = fieldDeclarationSyntax; node is object; node = node.Parent)
            {
                if (node is TypeDeclarationSyntax type)
                {
                    if (!type.Modifiers.Any(modifier => modifier.Text == "public"))
                        return false;
                }
            }
            return true;
        }
    }
}
