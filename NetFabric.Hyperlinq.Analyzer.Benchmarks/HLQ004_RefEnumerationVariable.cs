using BenchmarkDotNet.Attributes;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ004_RefEnumerationVariable
{
    public readonly record struct Person(string Name, int Age);

    Person[]? people;

    [Params(100, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        people = Enumerable.Range(0, Count)
            .Select(value => new Person(value.ToString(), value))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public Person? Copy()
    {
        var oldest = default(Person);
        foreach (var person in people!.AsSpan())
        {
            if (person.Age > oldest.Age)
                oldest = person;
        }
        return oldest;
    }

    [Benchmark]
    public Person Ref()
    {
        var oldest = default(Person);
        foreach (ref var person in people!.AsSpan())
        {
            if (person.Age > oldest.Age)
                oldest = person;
        }
        return oldest;
    }
}
