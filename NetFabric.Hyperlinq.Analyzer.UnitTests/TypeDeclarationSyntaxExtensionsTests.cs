using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class TypeDeclarationSyntaxExtensionsTests
    {
        [Theory]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(global::C))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(global::S))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(global::C.CC))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(global::C.CS))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(global::S.SC))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(global::S.SS))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(Outer.Inner.C))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(Outer.Inner.C.CC))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(First.Second.C))]
        [InlineData("TestData/TypeDeclarationSyntaxExtensions/Test.cs", typeof(First.Second.C.CC))]
        public void GetMetadataName_Should_ReturnMetadataName(string path, Type type)
        {
            // Arrange
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));

            // Act
            var result = tree.GetRoot()
                .DescendantNodes().OfType<TypeDeclarationSyntax>()
                .FirstOrDefault(node => node.GetMetadataName() == type.FullName);

            // Assert   
            Assert.NotNull(result);
        }
    }
}
