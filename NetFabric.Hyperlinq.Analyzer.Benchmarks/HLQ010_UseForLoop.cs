using BenchmarkDotNet.Attributes;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ010_UseForLoop
{
    List<int>? list;

    [Params(100, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        list = System.Linq.Enumerable.Range(0, Count).ToList();
    }

    [Benchmark(Baseline = true)]
    public int Foreach()
    {
        var sum = 0;
        foreach (var item in list!)
            sum += item;
        return sum;
    }

    [Benchmark]
    public int For()
    {
        var sum = 0;
        for (var index = 0; index < list!.Count; index++)
        {
            var item = list![index];
            sum += item;
        }
        return sum;
    }
}
