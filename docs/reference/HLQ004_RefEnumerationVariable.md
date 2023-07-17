# HLQ004: The enumerator returns a reference to the item.

## Cause

The enumerator returns a reference to the items. The enumeration variable is not declared as a reference so a copy of each item will be made.

## Severity

Warning

## Rule description

### Benchmarks

The following benchmarks compare the performance of the enumerator returning a reference to the item and the enumerator returning a copy of the item.

Source: https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/NetFabric.Hyperlinq.Analyzer.Benchmarks/HLQ004_RefEnumerationVariable.cs

```

BenchmarkDotNet v0.13.6, Windows 10 (10.0.19045.3269/22H2/2022Update)
Intel Core i7-7567U CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host] : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 6 : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7 : .NET 7.0.8 (7.0.823.31807), X64 RyuJIT AVX2
  .NET 8 : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2


```
| Method |    Job |  Runtime | Count |         Mean |      Error |     StdDev |       Median |        Ratio | RatioSD | Allocated | Alloc Ratio |
|------- |------- |--------- |------ |-------------:|-----------:|-----------:|-------------:|-------------:|--------:|----------:|------------:|
|   **Copy** | **.NET 6** | **.NET 6.0** |   **100** |    **194.52 ns** |  **13.354 ns** |  **39.373 ns** |    **193.28 ns** |     **baseline** |        **** |         **-** |          **NA** |
|    Ref | .NET 6 | .NET 6.0 |   100 |    106.87 ns |   4.638 ns |  13.676 ns |    104.42 ns | 1.86x faster |   0.48x |         - |          NA |
|        |        |          |       |              |            |            |              |              |         |           |             |
|   Copy | .NET 7 | .NET 7.0 |   100 |    107.09 ns |   3.389 ns |   9.669 ns |    105.55 ns |     baseline |         |         - |          NA |
|    Ref | .NET 7 | .NET 7.0 |   100 |     88.28 ns |   0.885 ns |   0.739 ns |     88.25 ns | 1.23x faster |   0.09x |         - |          NA |
|        |        |          |       |              |            |            |              |              |         |           |             |
|   Copy | .NET 8 | .NET 8.0 |   100 |    143.37 ns |   1.121 ns |   0.875 ns |    143.33 ns |     baseline |         |         - |          NA |
|    Ref | .NET 8 | .NET 8.0 |   100 |     74.84 ns |   3.139 ns |   9.007 ns |     69.55 ns | 1.98x faster |   0.19x |         - |          NA |
|        |        |          |       |              |            |            |              |              |         |           |             |
|   **Copy** | **.NET 6** | **.NET 6.0** | **10000** |  **8,137.04 ns** |  **50.933 ns** |  **47.643 ns** |  **8,131.04 ns** |     **baseline** |        **** |         **-** |          **NA** |
|    Ref | .NET 6 | .NET 6.0 | 10000 |  6,775.81 ns |  97.054 ns |  99.667 ns |  6,767.92 ns | 1.20x faster |   0.02x |         - |          NA |
|        |        |          |       |              |            |            |              |              |         |           |             |
|   Copy | .NET 7 | .NET 7.0 | 10000 | 13,655.17 ns | 219.651 ns | 183.418 ns | 13,596.77 ns |     baseline |         |         - |          NA |
|    Ref | .NET 7 | .NET 7.0 | 10000 |  8,157.08 ns | 154.674 ns | 151.911 ns |  8,109.30 ns | 1.68x faster |   0.02x |         - |          NA |
|        |        |          |       |              |            |            |              |              |         |           |             |
|   Copy | .NET 8 | .NET 8.0 | 10000 | 13,881.05 ns | 255.603 ns | 566.399 ns | 13,637.18 ns |     baseline |         |         - |          NA |
|    Ref | .NET 8 | .NET 8.0 | 10000 |  6,117.83 ns |  87.811 ns |  77.842 ns |  6,129.23 ns | 2.27x faster |   0.09x |         - |          NA |


## How to fix violations

Add the keywords 'ref' of 'ref readonly' before the 'var' keyword of variable type.

## When to suppress warnings

Should not be suppressed. 

## Example of a violation

```csharp
var span = new ReadOnlySpan<int>(new[] { 0, 1, 2, 3, 4, 5 });

foreach(var item in span)
    Console.WriteLine(item);
```

## Example of how to fix

```csharp
var span = new ReadOnlySpan<int>(new[] { 0, 1, 2, 3, 4, 5 });

foreach(ref readonly var item in span)
    Console.WriteLine(item);
```

