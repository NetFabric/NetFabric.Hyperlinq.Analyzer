using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance
            .WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend))
            .AddDiagnoser(MemoryDiagnoser.Default)
            //.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(
            //    printSource: true,
            //    exportGithubMarkdown: true)))
            .AddExporter(MarkdownExporter.GitHub)
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core70))
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80));

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
