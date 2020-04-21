# HLQ003: Public methods should return highest admissible level interfaces

## Cause

Public method returns a lower level interface than the one supported by the value returned.

## Severity

Warning

## Rule description

When returning a collection from a method, it's often a bad idea to return it type as it may provide capabilities you don't want it to. It's common to want it to be read-only.

The following interfaces give read-only views of a collection:

- [`IEnumerable<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) - allows sequential enumeration and can be used as the source for a `foreach` loop.

- [`IReadOnlyCollection<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1) - same as `IEnumerable<T>` plus a `Count` property with complexity *O(1)*.
- [`IReadOnlyList<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1) - same as `IReadOnlyCollection<T>` plus an indexer that allows; random access and the use of `for` loop, which is often more performant than `foreach`.
- [`IReadOnlyDictionary<TKey, TValue>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2) -  same as `IReadOnlyCollection<KeyValuePair<TKey, TValue>>` plus the read operations found in `Dictionary<TKey, TValue>`.

It's common to return `IEnumerable<T>` but the other interfaces allow the caller to take advantage of the other capabilities.

From the interfaces implemented by the collection you use internally, return the one that provides the most capabilities that are allowed.

## How to fix violations

Change the return type by the one suggested.

## When to suppress warnings

When, for contractual reasons, the return type cannot change.

## Example of a violation

```csharp
IEnumerable<int> Method()
{
    return new[] { 0, 1, 2, 4, 5 };
}
```

## Example of how to fix

```csharp
IReadOnlyList<int> Method()
{
    return new[] { 0, 1, 2, 4, 5 };
}
```
