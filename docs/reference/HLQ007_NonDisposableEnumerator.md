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

The `Dispose()` method is tipically implemented because it's part of the `IEnumerator<T>` contract but often is left empty.

`foreach` uses the type returned by the public `GetEnumerator()` method but this type doesn't have to implement `IEnumerator<T>`. 

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