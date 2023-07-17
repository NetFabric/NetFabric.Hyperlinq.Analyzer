# HLQ006: `GetEnumerator()` and `GetAsyncEnumerator()` should return an instance of a value-typed enumerator

## Cause

A `GetEnumerator()` or `GetAsyncEnumerator()` methods returns a reference type.

## Severity

Warning

## Rule description

Collections can be implemented so that the enumerator type returned by `GetEnumerator()` or `GetAsyncEnumerator()` is a value-type. The advantage is that the enumerator is allocated on the stack and method calls are not virtual.

### Benchmarks

The following benchmarks show the performance difference between a value-type enumerator and a reference-type enumerator. 

Source: https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/NetFabric.Hyperlinq.Analyzer.Benchmarks/HLQ006_GetEnumeratorReturnType.cs

```

BenchmarkDotNet v0.13.6, Windows 10 (10.0.19045.3269/22H2/2022Update)
Intel Core i7-7567U CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host] : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 6 : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7 : .NET 7.0.8 (7.0.823.31807), X64 RyuJIT AVX2
  .NET 8 : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2


```
|     Method |    Job |  Runtime | Count |         Mean |        Error |       StdDev |       Median |         Ratio | RatioSD |   Gen0 | Code Size | Allocated | Alloc Ratio |
|----------- |------- |--------- |------ |-------------:|-------------:|-------------:|-------------:|--------------:|--------:|-------:|----------:|----------:|------------:|
| **Enumerable** | **.NET 6** | **.NET 6.0** |   **100** |    **592.02 ns** |     **6.401 ns** |     **5.346 ns** |    **591.66 ns** |      **baseline** |        **** | **0.0153** |        **NA** |      **32 B** |            **** |
|  Optimized | .NET 6 | .NET 6.0 |   100 |     47.04 ns |     0.937 ns |     1.185 ns |     46.55 ns | 12.60x faster |   0.32x |      - |      51 B |         - |          NA |
|            |        |          |       |              |              |              |              |               |         |        |           |           |             |
| Enumerable | .NET 7 | .NET 7.0 |   100 |    294.39 ns |     4.180 ns |     3.705 ns |    292.82 ns |      baseline |         | 0.0153 |        NA |      32 B |             |
|  Optimized | .NET 7 | .NET 7.0 |   100 |     40.58 ns |     0.640 ns |     0.657 ns |     40.40 ns |  7.26x faster |   0.17x |      - |        NA |         - |          NA |
|            |        |          |       |              |              |              |              |               |         |        |           |           |             |
| Enumerable | .NET 8 | .NET 8.0 |   100 |    162.92 ns |     1.251 ns |     1.045 ns |    162.44 ns |      baseline |         | 0.0153 |        NA |      32 B |             |
|  Optimized | .NET 8 | .NET 8.0 |   100 |     40.38 ns |     0.852 ns |     1.759 ns |     39.54 ns |  3.98x faster |   0.21x |      - |        NA |         - |          NA |
|            |        |          |       |              |              |              |              |               |         |        |           |           |             |
| **Enumerable** | **.NET 6** | **.NET 6.0** | **10000** | **56,948.41 ns** | **1,307.058 ns** | **3,812.749 ns** | **54,513.15 ns** |      **baseline** |        **** |      **-** |        **NA** |      **32 B** |            **** |
|  Optimized | .NET 6 | .NET 6.0 | 10000 |  4,243.36 ns |    83.367 ns |   156.584 ns |  4,165.87 ns | 13.50x faster |   1.06x |      - |      51 B |         - |          NA |
|            |        |          |       |              |              |              |              |               |         |        |           |           |             |
| Enumerable | .NET 7 | .NET 7.0 | 10000 | 30,246.62 ns |   470.977 ns |   440.553 ns | 30,228.00 ns |      baseline |         |      - |        NA |      32 B |             |
|  Optimized | .NET 7 | .NET 7.0 | 10000 |  3,942.11 ns |    64.786 ns |    69.321 ns |  3,921.29 ns |  7.66x faster |   0.16x |      - |        NA |         - |          NA |
|            |        |          |       |              |              |              |              |               |         |        |           |           |             |
| Enumerable | .NET 8 | .NET 8.0 | 10000 | 14,688.73 ns |   289.924 ns |   322.249 ns | 14,564.94 ns |      baseline |         |      - |        NA |      32 B |             |
|  Optimized | .NET 8 | .NET 8.0 | 10000 |  3,564.98 ns |    67.489 ns |    66.283 ns |  3,549.58 ns |  4.13x faster |   0.13x |      - |        NA |         - |          NA |

## How to fix violations

Change the return type of the method to return the enumerator type. This may require the addition of interface methods to be explicitly implemented. 

## When to suppress warnings

When it's not feasible to create a value-type enumerator.

## Example of a violation

The following example shows implementations of `IEnumerable<T>` and `IAsyncEnumerable<T>` using reference-types:

```csharp
readonly struct Enumerable<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    class Enumerator : IEnumerator<T>
    {
        ...
    }
}

readonly struct AsyncEnumerable<T> : IAsyncEnumerable<T>
{
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) 
        => new Enumerator();

    class Enumerator : IAsyncEnumerator<T>
    {
        ...
    }
}
```

## Example of how to fix

Change the enumerator type declaration to be `public` and a `struct`. Change the `GetEnumerator()`and `GetAsyncEnumerator()` methods to return the enumerator type and not an interface. Add the necessary interface explicit method implementations:

```csharp
readonly struct Enumerable<T> : IEnumerable<T>
{
    public Enumerator GetEnumerator() => new Enumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    public struct Enumerator : IEnumerator<T>
    {
        ...
    }
}

readonly struct AsyncEnumerable<T> : IAsyncEnumerable<T>
{
    public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) 
        => new Enumerator();
    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) 
        => new Enumerator();

    public struct Enumerator : IAsyncEnumerator<T>
    {
        ...
    }
}

```

## Related rules

- [HLQ001: Assigment causes boxing of enumerator](https://github.com/NetFabric/NetFabric.Hyperlinq/tree/master/NetFabric.Hyperlinq.Analyzer/docs/reference/HLQ001_AssignmentBoxing.md)
- [HLQ007: Consider returning a non-disposable enumerator](https://github.com/NetFabric/NetFabric.Hyperlinq/tree/master/NetFabric.Hyperlinq.Analyzer/docs/reference/HLQ007_NonDisposableEnumerator.md)
