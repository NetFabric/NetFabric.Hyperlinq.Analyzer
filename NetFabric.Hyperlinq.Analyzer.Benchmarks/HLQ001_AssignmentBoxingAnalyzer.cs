using BenchmarkDotNet.Attributes;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ001_AssignmentBoxingAnalyzer
{
    List<int>? list;
    IEnumerable<int>? enumerable;

    [Params(100, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        list = System.Linq.Enumerable.Range(0, Count).ToList();
        enumerable = list;
    }

    [Benchmark(Baseline = true)]
    public int Enumerable()
    {
        var sum = 0;
        foreach (var item in enumerable!)
            sum += item;
        return sum;
    }

    [Benchmark]
    public int List()
    {
        var sum = 0;
        foreach (var item in list!)
            sum += item;
        return sum;
    }
}
