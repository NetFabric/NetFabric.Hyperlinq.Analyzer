# HLQ008: Consider adding `readonly`.

## Cause

An enumerable is a value type but not defined as `readonly`.

## Severity

Info

## Rule description

When the compiler doesn't know if a value type is immutable, it may create defensive copies when a member method is called. Adding the `readonly` modifier to the method definition, informs the compiler that the method does not change the internal state of the enumerable. In this case, the defensive copy is not required.

If the enumerable is immutable, it's better to add the `readonly` modifier to the type definition. It forces the fields to be `readonly` and sets all the methods are automatically set to `readonly`.

## How to fix violations

If the enumerable is immutable, add the `readonly` modifier to its definition.

## When to suppress warnings

When the enumerable is not immutable. 

You should then add the `readonly` modifier the methods that don't change the internal state of the enumerable.

## Example of a violation

```csharp
struct ValueTypeEnumerable<T>
{
    public Enumerator GetEnumerator() => new Enumerator();

    public struct Enumerator
    {
        public T Current => default;

        public bool MoveNext() => false;
    }
}
```

## Example of how to fix

```csharp
readonly struct ValueTypeEnumerable<T>
{
    public Enumerator GetEnumerator() => new Enumerator();

    public struct Enumerator
    {
        public T Current => default;

        public bool MoveNext() => false;
    }
}
```
