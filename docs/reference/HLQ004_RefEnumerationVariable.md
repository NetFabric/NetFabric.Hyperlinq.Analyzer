# HLQ004: The enumerator returns a reference to the item.

## Cause

The enumerator returns a reference to the items. The enumeration variable is not declared as a reference so a copy of each item will be made.

## Rule description

## How to fix violations

Add the keywords 'ref' of 'ref readonly' before the 'var' keyword of variable type.

## When to suppress warnings

Should not be suppressed. 

## Example of a violation

### Code

```csharp
foreach (var item in array.Where(_ => true))
{
}
```

## Example of how to fix

### Code

```csharp
foreach (ref readonly var item in array.Where(_ => true))
{
}
```

