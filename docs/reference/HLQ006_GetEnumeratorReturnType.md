# HLQ006: `GetEnumerator()` or `GetAsyncEnumerator()` should not return an interface type

A `GetEnumerator()` or `GetAsyncEnumerator()` methods returns an interface type.

## Rule description

Collections can be implemented so that the enumerator type returned by `GetEnumerator()` or `GetAsyncEnumerator()` is a value -type. The advantage is that the enumerator is allocated on the stack and method calls are not virtual.

Returning an interface type makes it always a reference type.

## How to fix violations

Change the return type of the method to return the enumerator type. This may require the addition of interface methods to be explicitly implemented. 

## When to suppress warnings

When it's not feasible to create a value-type enumerator.

## Example of a violation

The following example shows implementations of `IEnumerable<T>` and `IAsyncEnumerable<T>` that are detected by this rule:

```csharp
readonly struct Enumerable<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    struct Enumerator : IEnumerator<T>
    {
        ...
    }
}

readonly struct AsyncEnumerable<T> : IAsyncEnumerable<T>
{
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) 
        => new Enumerator();

    struct Enumerator : IAsyncEnumerator<T>
    {
        ...
    }
}
```

## Example of how to fix

Change the enumerator type declaration to be `public` and change the method return type. Add the necessary interface explicit implementations:

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
