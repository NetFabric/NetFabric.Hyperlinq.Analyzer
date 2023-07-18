using BenchmarkDotNet.Attributes;
using System.Runtime.InteropServices;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ012_UseCollectionsMarshalAsSpan
{
    List<int>? list;

    [Params(0, 10, 100, 1_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        list = Enumerable.Range(0, Count).ToList();
    }

    [Benchmark(Baseline = true)]
    public int ForEach()
    {
        var sum = 0;
        foreach (var angle in list!)
            sum += angle;
        return sum;
    }

    [Benchmark]
    public int For()
    {
        var sum = 0;
        for (var index = 0; index < list!.Count; index++)
            sum += list![index];
        return sum;
    }

    [Benchmark]
    public int ForEachAsSpan()
    {
        var sum = 0;
        foreach (var angle in CollectionsMarshal.AsSpan(list!))
            sum += angle;
        return sum;
    }
}
