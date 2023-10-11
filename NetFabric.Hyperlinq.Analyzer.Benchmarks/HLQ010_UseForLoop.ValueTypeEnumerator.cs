using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class HLQ010_UseForLoop_ValueTypeEnumerator
{
    ArraySegment<int> arraySegment;
    Dictionary<int, int>? dictionary;

    [Params(100, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var array = Enumerable.Range(0, Count).ToArray();
        arraySegment = new ArraySegment<int>(array);
        dictionary = array.ToDictionary(item => item);
    }

    [BenchmarkCategory("ArraySegment")]
    [Benchmark(Baseline = true)]
    public int ArraySegment_Foreach()
    {
        var sum = 0;
        foreach (var item in arraySegment)
            sum += item;
        return sum;
    }

    [BenchmarkCategory("ArraySegment")]
    [Benchmark]
    public int ArraySegment_For()
    {
        var sum = 0;
        for (var index = 0; index < arraySegment.Count; index++)
        {
            var item = arraySegment[index];
            sum += item;
        }
        return sum;
    }


    [BenchmarkCategory("Dictionary")]
    [Benchmark(Baseline = true)]
    public int Dictionary_Foreach()
    {
        var sum = 0;
        foreach (var item in dictionary!)
            sum += item.Value;
        return sum;
    }

    [BenchmarkCategory("Dictionary")]
    [Benchmark]
    public int Dictionary_For()
    {
        var sum = 0;
        for (var index = 0; index < dictionary!.Count; index++)
        {
            var item = dictionary![index];
            sum += item;
        }
        return sum;
    }
}
