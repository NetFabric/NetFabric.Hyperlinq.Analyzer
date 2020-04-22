# HLQ005: Avoid use of `Single()`, `SingleOrDefault()`, `SingleAsync()` and `SingleOrDefaultAsync()` operations

## Cause

Methods `Single()`, `SingleOrDefault()`, `SingleAsync()` or `SingleOrDefaultAsync()` are used to get the first element of a collection.

## Severity

Warning

## Rule description

The methods `Single()`, `SingleOrDefault()`, `First()` and `FirstOrDefault()` (plus the async counterparts) are typically used get the first element of a LINQ query, 

The methods `Single()`, `SingleOrDefault()`, `SingleAsync()` or `SingleOrDefaultAsync()` check if there is more than one element that satisfies the query conditions. This means that the source for the query has to be enumerated until a second element is found or, until the end. 

The methods `Single()`, `SingleOrDefault()`, `SingleAsync()` or `SingleOrDefaultAsync()` can reduce considerably the performance of a query. Validate data only when inputting it and not while querying it.

## How to fix violations

Use `First()`, `FirstOrDefault()`, `FirstAsync()` or `FirstOrDefaultAsync()` methods to get the first element of a query.

## When to suppress warnings

Suppress when unit testing, or validating data, and want to guarantee that the collection does not contain duplicates.

## Example of a violation

```csharp
public static Employee GetEmployee(this IEnumerable<Employee> employees, int employeeId)
    => employees.SingleOrDefault(employee => employee.Id == employeeId);
```

## Example of how to fix

```csharp
public static Employee GetEmployee(this IEnumerable<Employee> employees, int employeeId)
    => employees.FirstOrDefault(employee => employee.Id == employeeId);
```
