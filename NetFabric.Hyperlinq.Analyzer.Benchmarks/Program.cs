using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend))
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(
                printSource: true,
                exportGithubMarkdown: true)))
            .AddExporter(MarkdownExporter.GitHub)
            .AddJob(Job.Default
                .WithRuntime(CoreRuntime.Core60)
                .WithId(".NET 6"))
            .AddJob(Job.Default
                .WithRuntime(CoreRuntime.Core70)
                .WithId(".NET 7"))
            .AddJob(Job.Default
                .WithRuntime(CoreRuntime.Core80)
                .WithId(".NET 8"));

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
    }
}
