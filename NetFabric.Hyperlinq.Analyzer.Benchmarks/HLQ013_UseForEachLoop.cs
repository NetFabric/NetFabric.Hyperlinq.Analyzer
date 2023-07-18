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
        var length = array!.Length;
        for (var index = 0; index < length; index++)
            sum += array![index];
        return sum;
    }

    [Benchmark]
    public int ForEach_Array()
    {
        var sum = 0;
        foreach (var angle in array!)
            sum += angle;
        return sum;
    }

    [Benchmark]
    public int ForEach_Span()
    {
        var sum = 0;
        foreach (ref var angle in array!.AsSpan())
            sum += angle;
        return sum;
    }
}
