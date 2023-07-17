# HLQ012: Use `CollectionsMarshal.AsSpan()` to iterate `List<T>`.

## Cause

'foreach' loop is being used on a `List<T>`.

## Severity

Warning

## Rule description

When iterating over a `List<T>` in a `foreach` loop, the default behavior is to create an enumerator and allocate an iterator object. 
This incurs additional memory allocations and can impact performance, especially in scenarios with large collections or frequent iterations. 
By using the `CollectionsMarshal.AsSpan()` method, you can eliminate these overheads and directly access the underlying elements of the `List<T>` without creating an enumerator. 
Iterating a `Span<T>` directly avoids the overhead of the enumerator, resulting in faster loop iterations.

### Benchmarking

Source: https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/NetFabric.Hyperlinq.Analyzer.Benchmarks/HLQ012_UseCollectionsMarshalAsSpanAnalyzer.cs

```

BenchmarkDotNet v0.13.6, Windows 10 (10.0.19045.3155/22H2/2022Update)
Intel Core i7-7567U CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host] : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 6 : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7 : .NET 7.0.8 (7.0.823.31807), X64 RyuJIT AVX2
  .NET 8 : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2


```
|                   Method |    Job |  Runtime | Count |          Mean |      Error |     StdDev |        Median |        Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
|------------------------- |------- |--------- |------ |--------------:|-----------:|-----------:|--------------:|-------------:|--------:|----------:|----------:|------------:|
|                  **ForEach** | **.NET 6** | **.NET 6.0** |     **0** |     **1.0956 ns** |  **0.0530 ns** |  **0.1300 ns** |     **1.0414 ns** |     **baseline** |        **** |      **99 B** |         **-** |          **NA** |
| CollectionsMarshalAsSpan | .NET 6 | .NET 6.0 |     0 |     0.5871 ns |  0.0329 ns |  0.0379 ns |     0.5835 ns | 1.83x faster |   0.16x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 7 | .NET 7.0 |     0 |     1.3218 ns |  0.0395 ns |  0.0440 ns |     1.3141 ns |     baseline |         |        NA |         - |          NA |
| CollectionsMarshalAsSpan | .NET 7 | .NET 7.0 |     0 |     0.8831 ns |  0.0532 ns |  0.1510 ns |     0.8114 ns | 1.50x faster |   0.20x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 8 | .NET 8.0 |     0 |     0.0441 ns |  0.0310 ns |  0.0793 ns |     0.0000 ns |            ? |       ? |        NA |         - |           ? |
| CollectionsMarshalAsSpan | .NET 8 | .NET 8.0 |     0 |     0.4798 ns |  0.0416 ns |  0.1146 ns |     0.4198 ns |            ? |       ? |        NA |         - |           ? |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  **ForEach** | **.NET 6** | **.NET 6.0** |    **10** |    **14.2010 ns** |  **0.1204 ns** |  **0.1005 ns** |    **14.1930 ns** |     **baseline** |        **** |      **99 B** |         **-** |          **NA** |
| CollectionsMarshalAsSpan | .NET 6 | .NET 6.0 |    10 |     4.0248 ns |  0.1112 ns |  0.1189 ns |     3.9801 ns | 3.51x faster |   0.11x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 7 | .NET 7.0 |    10 |    10.3653 ns |  0.1658 ns |  0.1470 ns |    10.3078 ns |     baseline |         |        NA |         - |          NA |
| CollectionsMarshalAsSpan | .NET 7 | .NET 7.0 |    10 |     3.6369 ns |  0.1270 ns |  0.3583 ns |     3.4921 ns | 2.92x faster |   0.21x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 8 | .NET 8.0 |    10 |     7.1217 ns |  0.1269 ns |  0.1125 ns |     7.0938 ns |     baseline |         |        NA |         - |          NA |
| CollectionsMarshalAsSpan | .NET 8 | .NET 8.0 |    10 |     3.8439 ns |  0.1792 ns |  0.5084 ns |     3.6169 ns | 1.88x faster |   0.24x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  **ForEach** | **.NET 6** | **.NET 6.0** |   **100** |   **142.4614 ns** |  **2.6891 ns** |  **3.4966 ns** |   **142.0638 ns** |     **baseline** |        **** |      **99 B** |         **-** |          **NA** |
| CollectionsMarshalAsSpan | .NET 6 | .NET 6.0 |   100 |    46.1275 ns |  0.9312 ns |  0.9963 ns |    46.0060 ns | 3.09x faster |   0.12x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 7 | .NET 7.0 |   100 |   113.1277 ns |  2.2898 ns |  2.9773 ns |   111.7294 ns |     baseline |         |        NA |         - |          NA |
| CollectionsMarshalAsSpan | .NET 7 | .NET 7.0 |   100 |    37.8990 ns |  0.5435 ns |  0.5816 ns |    37.6491 ns | 3.00x faster |   0.10x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 8 | .NET 8.0 |   100 |    67.2375 ns |  1.3082 ns |  1.1596 ns |    66.9356 ns |     baseline |         |        NA |         - |          NA |
| CollectionsMarshalAsSpan | .NET 8 | .NET 8.0 |   100 |    39.8413 ns |  0.4387 ns |  0.4308 ns |    39.7005 ns | 1.69x faster |   0.04x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  **ForEach** | **.NET 6** | **.NET 6.0** |  **1000** | **1,316.1551 ns** |  **8.4594 ns** |  **6.6045 ns** | **1,315.7101 ns** |     **baseline** |        **** |      **99 B** |         **-** |          **NA** |
| CollectionsMarshalAsSpan | .NET 6 | .NET 6.0 |  1000 |   392.1319 ns |  6.4124 ns |  6.8612 ns |   388.7779 ns | 3.34x faster |   0.07x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 7 | .NET 7.0 |  1000 | 1,045.3751 ns |  5.2175 ns |  4.3569 ns | 1,045.0901 ns |     baseline |         |        NA |         - |          NA |
| CollectionsMarshalAsSpan | .NET 7 | .NET 7.0 |  1000 |   325.3411 ns |  2.4156 ns |  2.0171 ns |   325.4291 ns | 3.21x faster |   0.03x |        NA |         - |          NA |
|                          |        |          |       |               |            |            |               |              |         |           |           |             |
|                  ForEach | .NET 8 | .NET 8.0 |  1000 |   594.4719 ns |  7.7370 ns |  6.0406 ns |   593.9274 ns |     baseline |         |        NA |         - |          NA |
| CollectionsMarshalAsSpan | .NET 8 | .NET 8.0 |  1000 |   352.6162 ns |  5.7492 ns | 10.3669 ns |   348.4033 ns | 1.67x faster |   0.06x |        NA |         - |          NA |


## How to fix violations

Use the `CollectionsMarshal.AsSpan()` method to convert `List<T>` to a `Span<T>`.

## When to suppress warnings

The CollectionsMarshal.AsSpan() method is only available in .NET Standard 2.1 or later, and .NET Core 3.0 or later. Make sure your project targets a compatible framework version.

It's important to validate that using `CollectionsMarshal.AsSpan()` is suitable for your specific scenario. While it provides performance benefits, it may not be appropriate for all use cases.

## Example of a violation

```csharp
var source = new List<int>();
foreach (var item in source)
    Console.WriteLine(item);
```


## Example of how to fix

```csharp
var source = new List<int>();
foreach (var item in CollectionsMarshal.AsSpan(source))
    Console.WriteLine(item);
```
