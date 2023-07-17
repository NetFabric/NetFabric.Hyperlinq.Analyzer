# HLQ007: Consider returning a non-disposable enumerator

## Cause

The enumerator type returned by the public `GetEnumerator()` implements an empty `Dispose()` method.

## Severity

Warning

## Rule description

The following `foreach` loop

``` csharp
var list = new List<int>();
foreach (var item in list)
    Console.WriteLine(item);
```

is interpreted by the compiler as the following

``` csharp
List<int> list = new List<int>();
List<int>.Enumerator enumerator = list.GetEnumerator();
try
{
    while (enumerator.MoveNext())
    {
        int current = enumerator.Current;
        Console.WriteLine(current);
    }
}
finally
{
    ((IDisposable)enumerator).Dispose();
}
```

Notice the use of  `try` and `finally` so that the enumerator is disposed either an exception is thrown or not. Unfortunately this makes the `foreach` loop not inlinable, denying possible performance improvements.

The `Dispose()` method is typically implemented because it's part of the `IEnumerator<T>` contract but often is left empty.

`foreach` uses the type returned by the public `GetEnumerator()` method but this type doesn't have to implement `IEnumerator<T>`. 

### Benchmarks

Source: https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/NetFabric.Hyperlinq.Analyzer.Benchmarks/HLQ007_NonDisposableEnumerator.cs

```

BenchmarkDotNet v0.13.6, Windows 10 (10.0.19045.3269/22H2/2022Update)
Intel Core i7-7567U CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host] : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 6 : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7 : .NET 7.0.8 (7.0.823.31807), X64 RyuJIT AVX2
  .NET 8 : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2


```
|        Method |    Job |  Runtime |  Count |     Mean |    Error |   StdDev |   Median |        Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
|-------------- |------- |--------- |------- |---------:|---------:|---------:|---------:|-------------:|--------:|----------:|----------:|------------:|
|    Disposable | .NET 6 | .NET 6.0 | 100000 | 466.7 μs |  5.56 μs |  4.93 μs | 467.0 μs |     baseline |         |     102 B |       1 B |             |
| NonDisposable | .NET 6 | .NET 6.0 | 100000 | 409.8 μs |  6.57 μs |  7.57 μs | 408.4 μs | 1.14x faster |   0.03x |      72 B |         - |          NA |
|               |        |          |        |          |          |          |          |              |         |           |           |             |
|    Disposable | .NET 7 | .NET 7.0 | 100000 | 469.4 μs |  6.25 μs |  5.22 μs | 469.0 μs |     baseline |         |        NA |       1 B |             |
| NonDisposable | .NET 7 | .NET 7.0 | 100000 | 419.4 μs |  8.38 μs | 20.23 μs | 409.1 μs | 1.13x faster |   0.04x |      72 B |         - |          NA |
|               |        |          |        |          |          |          |          |              |         |           |           |             |
|    Disposable | .NET 8 | .NET 8.0 | 100000 | 490.9 μs | 11.32 μs | 32.28 μs | 475.8 μs |     baseline |         |        NA |         - |          NA |
| NonDisposable | .NET 8 | .NET 8.0 | 100000 | 429.6 μs |  8.55 μs | 23.69 μs | 419.1 μs | 1.14x faster |   0.10x |      69 B |         - |          NA |

## How to fix violations

When the `Dispose()` method is not mandatory, simply remove it.

If mandatory, because of a interface, implement one more enumerator type that doesn't have a `Dispose()` method and return it from the public  `GetEnumerator()` method. Return the disposable enumerator from other overloads implemented explicitly.

## When to suppress warnings

When the enumerable will never be called by a `foreach` or `GetEnumerator()` will only be called explicitly (not through interfaces).

## Example of a violation

```csharp
readonly struct Enumerable<T> : IEnumerable<T>
{
    public Enumerator GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    public struct Enumerator : IEnumerator<T>
    {
        T current;
        
        public T Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext() 
        {
            ...
        }

        public void Reset() => throw new NotImplementedException();

        public void Dispose() { } // empty dispose method
    }
}

```

## Example of how to fix

```csharp
readonly struct Enumerable<T> : IEnumerable<T>
{
    // return non-disposable enumerator
    public Enumerator GetEnumerator() => new Enumerator();
    // return disposable enumerator
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new DisposableEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator();

    // public non-disposable enumerator
    // should be value-type to improve performance
    // Reset() method is also ommited
    public struct Enumerator 
    {
        T current;
        
        public T Current => current;

        public bool MoveNext() 
        {
            ...
        }
    }

    // private disposable enumerator
    // should be reference type as it's always cast to interface 
    class DisposableEnumerator : IEnumerator<T> 
    {
        T current;
        
        public T Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext() 
        {
            ...
        }

        public void Reset() => throw new NotImplementedException();

        public void Dispose() { } // empty dispose method
    }
}

```

## Related rules

- [HLQ006: `GetEnumerator()` and `GetAsyncEnumerator()` should return an instance of a value-typed enumerator](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/docs/reference/HLQ006_GetEnumeratorReturnType.md)