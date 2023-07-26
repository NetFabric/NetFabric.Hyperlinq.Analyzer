using BenchmarkDotNet.Attributes;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ013_UseForEachLoop
{
    int[]? array;

    [Params(10, 1_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        array = Enumerable.Range(0, Count).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int For()
    {
        var sum = 0;
        for (var index = 0; index < Count; index++)
            sum += array![index];
        return sum;
    }

    [Benchmark]
    public int ForEach()
    {
        var sum = 0;
        foreach (var item in array!)
            sum += item;
        return sum;
    }

    [Benchmark]
    public int ForEach_Span()
    {
        var sum = 0;
        foreach (var item in array!.AsSpan()[..Count])
            sum += item;
        return sum;
    }
}
