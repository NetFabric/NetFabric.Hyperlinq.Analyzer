[![GitHub last commit (master)](https://img.shields.io/github/last-commit/NetFabric/NetFabric.Hyperlinq.Analyzer/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/commits/master)
[![Build (master)](https://img.shields.io/github/workflow/status/NetFabric/NetFabric.Hyperlinq.Analyzer/.NET%20Core/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/actions)
[![Coverage](https://img.shields.io/coveralls/github/NetFabric/NetFabric.Hyperlinq.Analyzer/master?style=flat-square&logo=coveralls)](https://coveralls.io/github/NetFabric/NetFabric.Hyperlinq.Analyzer)
[![NuGet Version](https://img.shields.io/nuget/v/NetFabric.Hyperlinq.Analyzer.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NetFabric.Hyperlinq.Analyzer/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.Hyperlinq.Analyzer.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NetFabric.Hyperlinq.Analyzer/) 
[![Gitter](https://img.shields.io/gitter/room/netfabric/netfabric.hyperlinq.analyzer?style=flat-square&logo=gitter)](https://gitter.im/NetFabric/NetFabric.Hyperlinq.Analyzer)

# NetFabric.Hyperlinq.Analyzer

A Roslyn Analyzer that contains several enumeration-related rules to help users improve performance.

The analyzer is independent of `NetFabric.Hyperlinq`. The rules are useful even if you only use `IEnumerable<T>` or `System.Linq`.

Check the documentation for the implemented rules at https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference

# References

- [Enumeration in .NET](https://blog.usejournal.com/enumeration-in-net-d5674921512e) by Antão Almada

## Credits

The following open-source projects are used to build and test this project:

- [.NET](https://github.com/dotnet)
- [coveralls](https://coveralls.io)
- [coverlet](https://github.com/tonerdo/coverlet)
- [ILRepack](https://github.com/gluck/il-repack)
- [ILRepack.MSBuild.Task](https://github.com/peters/ILRepack.MSBuild.Task)
- [NetFabric.CodeAnalysis](https://github.com/NetFabric/NetFabric.CodeAnalysis)
- [xUnit.net](https://xunit.net/)
