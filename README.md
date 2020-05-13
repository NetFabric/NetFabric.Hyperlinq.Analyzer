[![GitHub last commit (master)](https://img.shields.io/github/last-commit/NetFabric/NetFabric.Hyperlinq.Analyzer/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/commits/master)
[![Build (master)](https://img.shields.io/github/workflow/status/NetFabric/NetFabric.Hyperlinq.Analyzer/.NET%20Core/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/actions)
[![Coverage](https://img.shields.io/coveralls/github/NetFabric/NetFabric.Hyperlinq.Analyzer/master?style=flat-square&logo=coveralls)](https://coveralls.io/github/NetFabric/NetFabric.Hyperlinq.Analyzer)
[![NuGet Version](https://img.shields.io/nuget/v/NetFabric.Hyperlinq.Analyzer.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NetFabric.Hyperlinq.Analyzer/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.Hyperlinq.Analyzer.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NetFabric.Hyperlinq.Analyzer/) 
[![Gitter](https://img.shields.io/gitter/room/netfabric/netfabric.hyperlinq.analyzer?style=flat-square&logo=gitter)](https://gitter.im/NetFabric/NetFabric.Hyperlinq.Analyzer)

# NetFabric.Hyperlinq.Analyzer

A [Roslyn Analyzer](https://docs.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview) that contains several rules to help improve enumeration performance when using C#.

**Note:** This analyzer is independent of [`NetFabric.Hyperlinq`](https://github.com/NetFabric/NetFabric.Hyperlinq). The rules may be useful when you only use `foreach`, `IEnumerable<T>`, `IAsyncEnumerable<T>`, `System.Linq` or `System.Linq.Async`.

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
[HLQ001](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ001_AssignmentBoxing.md)  | Performance | Warning  | Assigment to interface causes boxing of enumerator 
[HLQ002](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ002_NullEnumerable.md)  | Compiler | Error | Enumerable cannot be null. 
[HLQ003](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ003_HighestLevelInterface.md)  | Performance | Warning  | Public methods should return highest admissible level interface. 
[HLQ004](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ004_RefEnumerationVariable.md)  | Performance | Warning  | The enumerator returns a reference to the item. 
[HLQ005](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ005_AvoidSingle.md)  | Performance | Warning  | Avoid use of Single() and SingleOrDefault()
[HLQ006](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ006_GetEnumeratorReturnType.md)  | Performance | Warning  | GetEnumerator() or GetAsyncEnumerator() should return a value type. 
[HLQ007](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ007_NonDisposableEnumerator.md)  | Performance |  Warning | Consider returning a non-disposable enumerator.
[HLQ008](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ008_ReadOnlyRefEnumerable.md)  | Performance |  Info | The enumerable is a value type. Consider making it 'readonly'.
[HLQ009](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ009_RemoveOptionalMethods.md)  | Performance |  Info | Consider removing an empty optional enumerator method.
[HLQ010](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ010_UseForLoop.md)  | Performance |  Warning | Consider using a 'for' loop instead.


# Usage

Add the [NetFabric.Hyperlinq.Analyzer](https://www.nuget.org/packages/NetFabric.Hyperlinq.Analyzer/) package to your project using your favorite NuGet client.

If added manually to the `.csproj`, make sure to set `PrivateAssets` to `all` so that it's not added as a dependency. A [floating version](https://docs.microsoft.com/en-us/nuget/concepts/dependency-resolution#floating-versions) can be used to get the latest version. 

``` xml
<PackageReference Include="NetFabric.Hyperlinq.Analyzer" Version="1.*">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
```

# References

- [Enumeration in .NET](https://blog.usejournal.com/enumeration-in-net-d5674921512e) by Antão Almada
- [NetFabric.Hyperlinq — Optimizing LINQ](https://medium.com/@antao.almada/netfabric-hyperlinq-optimizing-linq-348e02566cef) by Antão Almada
- [Unit testing a Roslyn Analyzer](https://medium.com/@antao.almada/unit-testing-a-roslyn-analyzer-b3da666f0252) by Antão Almada

## Credits

The following open-source projects are used to build and test this project:

- [.NET](https://github.com/dotnet)
- [coveralls](https://coveralls.io)
- [coverlet](https://github.com/tonerdo/coverlet)
- [ILRepack](https://github.com/gluck/il-repack)
- [ILRepack.MSBuild.Task](https://github.com/peters/ILRepack.MSBuild.Task)
- [NetFabric.CodeAnalysis](https://github.com/NetFabric/NetFabric.CodeAnalysis)
- [xUnit.net](https://xunit.net/)
