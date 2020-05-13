# HLQ002: Enumerable methods should not return null

## Cause

A method that returns an enumerable type is returning `null`.

## Severity

Error

## Rule description

The following `foreach` loop: 

```csharp
foreach (var item in DoSomething())
    Console.WriteLine(item);
```

is interpreted by the compiler as:

```csharp
IEnumerator<int> enumerator = DoSomething().GetEnumerator();
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
    if (enumerator != null)
    {
        enumerator.Dispose();
    }
}
```

If the method `DoSomething()` returns `null`, [a `NullReferenceException` will be thrown](https://sharplab.io/#v2:C4LgTgrgdgNAJiA1AHwAIAYAEqCMBuAWACgNscAWQo41AZjIDZsAmMgdk2IG9jM/t6uJqnKYAsgEMAllAAUASk5F+mHspX8AZgHswAUwkBjABaZZANwlhMU4HoC2NqJgDie4AFEoEe3rASAIwAbPQV5Xg1I3ABOWVsHeSoVAF8IvjTGMloAHhlgAD5Xdy8fP0CQhUwAXkLvIKCqZKA==) when trying to call `GetEnumerator()`.

The same issue applies to `IAsyncEnumerator<T>`.

`null` is not equivalent to an empty enumerable. An empty collection, is a collection where calls to its `MoveNext()` returns always `false`. While `null` is an invalid state.

## How to fix violations

Return an instance of an empty collection.

## When to suppress warnings

Should not be suppressed.

## Example of a violation

```csharp
IEnumerable<int> Method()
{
    if (...)
        return null;

    ...
}
```

```csharp
IAsyncEnumerable<int> Method()
{
    if (...)
        return null;

    ...
}
```

## Example of how to fix

Using an empty array:

```csharp
using System;

IEnumerable<int> Method()
{
    if (...)
        return Array.Empty<int>();

    ...
}
```

Using an empty `List<T>`:

```csharp
using System.Collections.Generic;

IEnumerable<int> Method()
{
    if (...)
        return new List<int>();

    ...
}
```

Using `Enumerable.Empty<int>()`:

```csharp
using System.Linq;

IEnumerable<int> Method()
{
    if (...)
        return Enumerable.Empty<int>();

    ...
}
```

Using `AsyncEnumerable.Empty<int>()` (requires [System.Linq.Async](https://www.nuget.org/packages/System.Linq.Async/) package):

```csharp
using System.Linq.Async;

IAsyncEnumerable<int> Method()
{
    if (...)
        return AsyncEnumerable.Empty<int>();

    ...
}
```

