using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class HLQ005_AvoidSingleAnalyzer
{
    int[]? bestCase;
    int[]? worstCase;

    [Params(100, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        bestCase = Enumerable.Range(0, Count).ToArray();
        worstCase = bestCase.Reverse().ToArray();
    }

    [BenchmarkCategory("BestCase")]
    [Benchmark(Baseline = true)]
    public int BestCase_Single()
        => bestCase!.Single(Comparer);

    [BenchmarkCategory("BestCase")]
    [Benchmark]
    public int BestCase_First()
        => bestCase!.First(Comparer);

    [BenchmarkCategory("WorstCase")]
    [Benchmark(Baseline = true)]
    public int WorstCase_Single()
        => worstCase!.Single(Comparer);

    [BenchmarkCategory("WorstCase")]
    [Benchmark]
    public int WorstCase_First()
        => worstCase!.First(Comparer);

    static bool Comparer(int value) 
        => value == 0;
}
