# HLQ005: Avoid `Single()` and `SingleOrDefault()`

## Cause

Methods `Single()` or `SingleOrDefault()` are used to get the first element of a collection.

## Severity

Warning

## Rule description

The methods `Single()`, `SingleOrDefault()`, `First()` and `FirstOrDefault()` are tipically used at the end of a LINQ query to get only the first element. 

They throw an exception or return default when the collection is empty.

The methods `Single()` and `SingleOrDefault()` also check if there is one more element that satisfies the query conditions. This means that the query has to be performed to all subsequent elements until a second one if found. If none is found, the original collection has to be completely enumerated.

The methods `Single()` and `SingleOrDefault()` can increase considerably the execution time of a query.

## How to fix violations

Use `First()` or `FirstOrDefault()` methods instead to get the first element of a collection.

## When to suppress warnings

Supress when unit testing or validating data.

## Example of a violation

`GetEmployee()` is an extension method that return an employee with a given `employeeId` from an `IEnumerable<Employee>`. Returns `null` if employee not found.
Throws an exception if more than one employee found.

```csharp
public static Employee GetEmployee(this IEnumerable<Employee> employees, int employeeId)
    => employees.SingleOrDefault(employee => employee.Id == employeeId);
```

## Example of how to fix

Refactor data validation logic so that it's more efficient. Replace `SingleOrDefault` by `FirstOrDefault`. 

```csharp
public static Employee GetEmployee(this IEnumerable<Employee> employees, int employeeId)
    => employees.FirstOrDefault(employee => employee.Id == employeeId);
```
