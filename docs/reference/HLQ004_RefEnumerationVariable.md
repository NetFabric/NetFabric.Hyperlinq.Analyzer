# HLQ004: The enumerator returns a reference to the item.

## Cause

The enumerator returns a reference to the items. The enumeration variable is not declared as a reference so a copy of each item will be made.

## Severity

Warning

## Rule description

## How to fix violations

Add the keywords 'ref' of 'ref readonly' before the 'var' keyword of variable type.

## When to suppress warnings

Should not be suppressed. 

## Example of a violation

```csharp
var span = new ReadOnlySpan<int>(new[] { 0, 1, 2, 3, 4, 5 });

foreach(var item in span)
    Console.WriteLine(item);
```

## Example of how to fix

```csharp
var span = new ReadOnlySpan<int>(new[] { 0, 1, 2, 3, 4, 5 });

foreach(ref readonly var item in span) // add 'ref readonly' here
    Console.WriteLine(item);
```

