# HLQ010: Use 'for' loop

## Cause

'foreach' loop is being used on a enumerable that has an indexer.

## Severity

Warning

## Rule description

When the collecton is an array, a 'foreach' loop is internally converted into a 'for'. This happens because the indexer of an array is a lot more efficient that the its enumerator.

The indexer is usually more efficient but the compiler cannot make that assumption so, it doesn't make the conversion for other types.

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
