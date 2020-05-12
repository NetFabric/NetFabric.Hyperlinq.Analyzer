# HLQ009: Consider removing an empty optional enumerator method.

## Cause

A method is empty and is only required when an enumerator interface is implemented.

## Severity

Info

## Rule description

The methods `Reset`, `Dispose` and `DisposeAsync` are not required for an enumerator to be valid for a `foreach`. These are only required when the enumerator implements `IEnumerator`, `IEnunerator<>` or `IAsyncEnumerator<>`. 

## How to fix violations

If the method is empty and none of the mentioned interfaces is implemented then consider removing it.

## When to suppress warnings

When method is part of some other contract.

## Example of a violation

```csharp
readonly struct Enumerable<T>
{
    public Enumerator GetEnumerator() => new Enumerator();

    // not enumerator interfaces implemented
    public struct Enumerator
    {
        public T Current => default;

        public bool MoveNext() => false;

        // empty Reset method
        public void Reset() => throw new NotImplementedException();

        // empty Dispose method
        public void Dispose() { }
    }
}
```

## Example of how to fix

```csharp
readonly struct Enumerable<T>
{
    public Enumerator GetEnumerator() => new Enumerator();

    public struct Enumerator
    {
        public T Current => default;

        public bool MoveNext() => false;
    }
}
```
