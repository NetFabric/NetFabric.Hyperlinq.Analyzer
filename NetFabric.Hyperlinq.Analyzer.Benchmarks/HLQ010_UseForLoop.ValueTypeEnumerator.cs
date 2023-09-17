using BenchmarkDotNet.Attributes;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ010_UseForLoop_ValueTypeEnumerator
{
    ArraySegment<int> source;

    [Params(100, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        source = new ArraySegment<int>(Enumerable.Range(0, Count).ToArray());
    }

    [Benchmark(Baseline = true)]
    public int Foreach()
    {
        var sum = 0;
        foreach (var item in source!)
            sum += item;
        return sum;
    }

    [Benchmark]
    public int For()
    {
        var sum = 0;
        for (var index = 0; index < source!.Count; index++)
        {
            var item = source![index];
            sum += item;
        }
        return sum;
    }
}
