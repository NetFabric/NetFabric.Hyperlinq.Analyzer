# HLQ005: Avoid use of `Single()`, `SingleOrDefault()`, `SingleAsync()` and `SingleOrDefaultAsync()` operations

## Cause

Methods `Single()`, `SingleOrDefault()`, `SingleAsync()` or `SingleOrDefaultAsync()` are used to get the first element of a collection.

## Severity

Warning

## Rule description

The methods `Single()`, `SingleOrDefault()`, `First()` and `FirstOrDefault()` (plus the async counterparts) are typically used get the first element of a LINQ query. Verifying if the result of the query contains a single item requires applying the query to the souce items until a second item is found or, until the end of the collection. This can be a very expensive operation.

If the query returns more than one item and it's not expected, there is either a problem with the query of in the source collection. Verifying these conditions at runtime is not a good practice. 

### Benchmarks

Comparing the performance of `Single()` and `First()` for the best case scenario, where the first element of the collection satisfies the query conditions, and the worst case, where only the last element of the collection does not satisfy the query conditions.

Source: https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/blob/master/NetFabric.Hyperlinq.Analyzer.Benchmarks/HLQ005_AvoidSingleAnalyzer.cs

``` ini

BenchmarkDotNet v0.13.6, Windows 10 (10.0.19045.3269/22H2/2022Update)
Intel Core i7-7567U CPU 3.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100-preview.5.23303.2
  [Host] : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 6 : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7 : .NET 7.0.8 (7.0.823.31807), X64 RyuJIT AVX2
  .NET 8 : .NET 8.0.0 (8.0.23.28008), X64 RyuJIT AVX2


```
|           Method |    Job |  Runtime | Categories | Count |         Mean |        Error |       StdDev |       Median |             Ratio | RatioSD |   Gen0 | Code Size | Allocated | Alloc Ratio |
|----------------- |------- |--------- |----------- |------ |-------------:|-------------:|-------------:|-------------:|------------------:|--------:|-------:|----------:|----------:|------------:|
|  **BestCase_Single** | **.NET 6** | **.NET 6.0** |   **BestCase** |   **100** |    **620.48 ns** |    **10.790 ns** |    **13.646 ns** |    **616.74 ns** |          **baseline** |        **** | **0.0153** |     **547 B** |      **32 B** |            **** |
|   BestCase_First | .NET 6 | .NET 6.0 |   BestCase |   100 |     17.92 ns |     0.197 ns |     0.242 ns |     17.94 ns |     34.66x faster |   0.72x | 0.0153 |     456 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
|  BestCase_Single | .NET 7 | .NET 7.0 |   BestCase |   100 |    647.87 ns |     7.507 ns |     5.861 ns |    649.43 ns |          baseline |         | 0.0153 |     510 B |      32 B |             |
|   BestCase_First | .NET 7 | .NET 7.0 |   BestCase |   100 |     19.52 ns |     0.371 ns |     0.329 ns |     19.51 ns |     33.19x faster |   0.69x | 0.0153 |     430 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
|  BestCase_Single | .NET 8 | .NET 8.0 |   BestCase |   100 |    287.77 ns |     3.058 ns |     2.554 ns |    287.55 ns |          baseline |         | 0.0153 |   1,036 B |      32 B |             |
|   BestCase_First | .NET 8 | .NET 8.0 |   BestCase |   100 |     13.98 ns |     0.145 ns |     0.129 ns |     13.97 ns |     20.57x faster |   0.20x | 0.0153 |     659 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
|  **BestCase_Single** | **.NET 6** | **.NET 6.0** |   **BestCase** | **10000** | **59,386.15 ns** |   **428.718 ns** |   **401.023 ns** | **59,392.95 ns** |          **baseline** |        **** |      **-** |     **547 B** |      **32 B** |            **** |
|   BestCase_First | .NET 6 | .NET 6.0 |   BestCase | 10000 |     18.41 ns |     0.272 ns |     0.213 ns |     18.34 ns | 3,225.926x faster |  49.88x | 0.0153 |     456 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
|  BestCase_Single | .NET 7 | .NET 7.0 |   BestCase | 10000 | 59,037.31 ns |   333.439 ns |   260.327 ns | 59,069.24 ns |          baseline |         |      - |     510 B |      32 B |             |
|   BestCase_First | .NET 7 | .NET 7.0 |   BestCase | 10000 |     20.71 ns |     0.955 ns |     2.710 ns |     19.22 ns | 2,894.540x faster | 284.74x | 0.0153 |     430 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
|  BestCase_Single | .NET 8 | .NET 8.0 |   BestCase | 10000 | 31,992.80 ns |   280.590 ns |   219.066 ns | 31,943.08 ns |          baseline |         |      - |     883 B |      32 B |             |
|   BestCase_First | .NET 8 | .NET 8.0 |   BestCase | 10000 |     13.83 ns |     0.308 ns |     0.400 ns |     13.75 ns | 2,332.087x faster |  61.31x | 0.0153 |     660 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
| **WorstCase_Single** | **.NET 6** | **.NET 6.0** |  **WorstCase** |   **100** |    **603.35 ns** |     **9.153 ns** |     **7.643 ns** |    **601.27 ns** |          **baseline** |        **** | **0.0153** |     **547 B** |      **32 B** |            **** |
|  WorstCase_First | .NET 6 | .NET 6.0 |  WorstCase |   100 |    620.98 ns |     6.142 ns |     5.129 ns |    620.11 ns |      1.03x slower |   0.02x | 0.0153 |     456 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
| WorstCase_Single | .NET 7 | .NET 7.0 |  WorstCase |   100 |    644.13 ns |     5.286 ns |     5.875 ns |    642.98 ns |          baseline |         | 0.0153 |     510 B |      32 B |             |
|  WorstCase_First | .NET 7 | .NET 7.0 |  WorstCase |   100 |    646.30 ns |    10.952 ns |     8.550 ns |    644.88 ns |      1.00x slower |   0.02x | 0.0153 |     430 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
| WorstCase_Single | .NET 8 | .NET 8.0 |  WorstCase |   100 |    287.32 ns |     2.771 ns |     2.457 ns |    286.77 ns |          baseline |         | 0.0153 |     826 B |      32 B |             |
|  WorstCase_First | .NET 8 | .NET 8.0 |  WorstCase |   100 |    307.45 ns |     3.885 ns |     4.914 ns |    306.72 ns |      1.07x slower |   0.02x | 0.0153 |     639 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
| **WorstCase_Single** | **.NET 6** | **.NET 6.0** |  **WorstCase** | **10000** | **57,425.36 ns** | **1,099.017 ns** | **1,079.382 ns** | **57,099.79 ns** |          **baseline** |        **** |      **-** |     **547 B** |      **32 B** |            **** |
|  WorstCase_First | .NET 6 | .NET 6.0 |  WorstCase | 10000 | 60,242.67 ns | 1,124.441 ns | 1,998.693 ns | 59,291.72 ns |      1.05x slower |   0.04x |      - |     456 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
| WorstCase_Single | .NET 7 | .NET 7.0 |  WorstCase | 10000 | 61,896.58 ns |   647.584 ns |   636.014 ns | 61,721.18 ns |          baseline |         |      - |     510 B |      32 B |             |
|  WorstCase_First | .NET 7 | .NET 7.0 |  WorstCase | 10000 | 62,517.62 ns |   945.663 ns | 1,776.185 ns | 61,873.28 ns |      1.02x slower |   0.04x |      - |     430 B |      32 B |  1.00x more |
|                  |        |          |            |       |              |              |              |              |                   |         |        |           |           |             |
| WorstCase_Single | .NET 8 | .NET 8.0 |  WorstCase | 10000 | 26,906.21 ns |   331.350 ns |   309.945 ns | 26,867.91 ns |          baseline |         |      - |     765 B |      32 B |             |
|  WorstCase_First | .NET 8 | .NET 8.0 |  WorstCase | 10000 | 30,013.80 ns |   593.585 ns | 1,252.072 ns | 29,412.80 ns |      1.12x slower |   0.05x |      - |     594 B |      32 B |  1.00x more |


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
