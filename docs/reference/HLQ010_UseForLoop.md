# HLQ010: Use 'for' loop

## Cause

'foreach' loop is being used on a enumerable that has an indexer.

## Severity

Warning

## Rule description

When the collecton is an array, a 'foreach' loop is internally converted into a 'for'. This happens because the indexer of an array is a lot more efficient that the its enumerator.

The indexer is usually more efficient but the compiler cannot make that assumption so, it doesn't make the conversion for other types.

### Benchmarks

Source: https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/NetFabric.Hyperlinq.Analyzer.Benchmarks/HLQ010_UseForLoop.cs

```

BenchmarkDotNet v0.13.6, Windows 10 (10.0.19045.3269/22H2/2022Update)
Intel Core i7-7567U CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host] : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 6 : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7 : .NET 7.0.8 (7.0.823.31807), X64 RyuJIT AVX2
  .NET 8 : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2


```
|  Method |    Job |  Runtime | Count |         Mean |      Error |     StdDev |       Median |        Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
|-------- |------- |--------- |------ |-------------:|-----------:|-----------:|-------------:|-------------:|--------:|----------:|----------:|------------:|
| **Foreach** | **.NET 6** | **.NET 6.0** |   **100** |    **179.92 ns** |  **13.386 ns** |  **39.468 ns** |    **161.90 ns** |     **baseline** |        **** |      **99 B** |         **-** |          **NA** |
|     For | .NET 6 | .NET 6.0 |   100 |    104.88 ns |   6.324 ns |  18.648 ns |     98.49 ns | 1.76x faster |   0.46x |      77 B |         - |          NA |
|         |        |          |       |              |            |            |              |              |         |           |           |             |
| Foreach | .NET 7 | .NET 7.0 |   100 |    121.68 ns |   2.565 ns |   7.318 ns |    119.04 ns |     baseline |         |        NA |         - |          NA |
|     For | .NET 7 | .NET 7.0 |   100 |     61.44 ns |   0.571 ns |   0.446 ns |     61.32 ns | 2.03x faster |   0.14x |      73 B |         - |          NA |
|         |        |          |       |              |            |            |              |              |         |           |           |             |
| Foreach | .NET 8 | .NET 8.0 |   100 |     71.27 ns |   1.464 ns |   2.991 ns |     69.95 ns |     baseline |         |        NA |         - |          NA |
|     For | .NET 8 | .NET 8.0 |   100 |     61.76 ns |   1.079 ns |   1.009 ns |     61.26 ns | 1.16x faster |   0.06x |      72 B |         - |          NA |
|         |        |          |       |              |            |            |              |              |         |           |           |             |
| **Foreach** | **.NET 6** | **.NET 6.0** | **10000** | **13,881.17 ns** | **271.842 ns** | **302.151 ns** | **13,802.66 ns** |     **baseline** |        **** |      **99 B** |         **-** |          **NA** |
|     For | .NET 6 | .NET 6.0 | 10000 |  8,177.57 ns | 161.223 ns | 125.872 ns |  8,184.67 ns | 1.70x faster |   0.03x |      77 B |         - |          NA |
|         |        |          |       |              |            |            |              |              |         |           |           |             |
| Foreach | .NET 7 | .NET 7.0 | 10000 | 10,779.54 ns |  81.614 ns |  63.719 ns | 10,784.76 ns |     baseline |         |        NA |         - |          NA |
|     For | .NET 7 | .NET 7.0 | 10000 |  5,701.41 ns | 109.829 ns | 102.734 ns |  5,664.83 ns | 1.89x faster |   0.04x |      73 B |         - |          NA |
|         |        |          |       |              |            |            |              |              |         |           |           |             |
| Foreach | .NET 8 | .NET 8.0 | 10000 |  5,662.48 ns |  68.151 ns |  56.909 ns |  5,651.46 ns |     baseline |         |        NA |         - |          NA |
|     For | .NET 8 | .NET 8.0 | 10000 |  5,658.66 ns | 112.406 ns | 275.733 ns |  5,510.43 ns | 1.01x slower |   0.06x |      72 B |         - |          NA |


## How to fix violations

Replace the 'foreach' loop for a 'for' loop.

## When to suppress warnings

When it's known that the indexer is not more efficient.

## Example of a violation

```csharp
var list = new List<int>(new[] { 0, 1, 2, 3, 4, 5 });

foreach (var item in list) // using the enumerator
    Console.WriteLine(item);
```

## Example of how to fix

```csharp
var list = new List<int>(new[] { 0, 1, 2, 3, 4, 5 });

for (var index = 0; index < list.Count; index++)
{
    var item = list[index]; // using the indexer
    Console.WriteLine(item);
}
```
