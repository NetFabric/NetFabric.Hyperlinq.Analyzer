using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class MethodDeclarationSyntaxExtensionsTests
    {
        [Theory]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/ReturnsVoid.cs", nameof(global::ReturnsVoid.ReturnsVoidMethod), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/ReturnsVoid.cs", nameof(global::ReturnsVoid.ReturnsTypeMethod), false)]
        public void ReturnsVoid_Should_Succeed(string path, string methodName, bool expected)
        {
            // Arrange
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
            var method = tree.GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ValueText == methodName);

            // Act
            var result = method.ReturnsVoid();

            // Assert   
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.EmptyArrowDefaultAsync), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.EmptyArrowNewAsync), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.EmptyArrowThrowAsync), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.EmptyBlockDefaultAsync), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.EmptyBlockNewAsync), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.EmptyBlockThrowAsync), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.NotEmptyBlockDefaultAsync), false)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.NotEmptyBlockNewAsync), false)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyAsyncMethod.cs", nameof(global::IsEmptyAsyncMethod.NotEmptyBlockThrowAsync), false)]
        public void IsEmptyAsyncMethod_Should_Succeed(string path, string methodName, bool expected)
        {
            // Arrange
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
            var method = tree.GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ValueText == methodName);

            // Act
            var result = method.IsEmptyAsyncMethod();

            // Assert   
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyMethod.cs", nameof(global::IsEmptyMethod.EmptyArrowThrow), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyMethod.cs", nameof(global::IsEmptyMethod.EmptyBlock), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyMethod.cs", nameof(global::IsEmptyMethod.EmptyBlockThrow), true)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyMethod.cs", nameof(global::IsEmptyMethod.NotEmptyBlock), false)]
        [InlineData("TestData/MethodDeclarationSyntaxExtensions/IsEmptyMethod.cs", nameof(global::IsEmptyMethod.NotEmptyBlockThrow), false)]
        public void IsEmptyMethod_Should_Succeed(string path, string methodName, bool expected)
        {
            // Arrange
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
            var method = tree.GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => method.Identifier.ValueText == methodName);

            // Act
            var result = method.IsEmptyMethod();

            // Assert   
            Assert.Equal(expected, result);
        }
    }
}
