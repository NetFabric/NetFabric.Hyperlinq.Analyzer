using BenchmarkDotNet.Attributes;
using System.Collections;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ006_GetEnumeratorReturnType
{
    TestEnumerable<int>? enumerable;
    TestEnumerableOptimized<int>? enumerableOptimized;

    [Params(100, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var array = System.Linq.Enumerable.Range(0, Count)
            .ToArray();
        enumerable = new(array);
        enumerableOptimized = new(array);
    }

    [Benchmark(Baseline = true)]
    public int Enumerable()
    {
        var sum = 0;
        foreach (var value in enumerable!)
            sum += value;
        return sum;
    }

    [Benchmark]
    public int Optimized()
    {
        var sum = 0;
        foreach (var value in enumerableOptimized!)
            sum += value;
        return sum;
    }

    public class TestEnumerable<T> 
        : IEnumerable<T>
    {
        readonly T[] array;

        public TestEnumerable(T[] array) 
            => this.array = array;

        public IEnumerator<T> GetEnumerator()
            => new Enumerator<T>(array);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public class TestEnumerableOptimized<T>
        : IEnumerable<T>
    {
        readonly T[] array;

        public TestEnumerableOptimized(T[] array) 
            => this.array = array;

        public Enumerator<T> GetEnumerator()
            => new(array);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public struct Enumerator<T>
        : IEnumerator<T>
    {
        readonly T[] array;
        int index;

        public Enumerator(T[] array)
        {
            this.array = array;
            index = -1;
        }

        public readonly T Current
            => array[index];

        readonly object? IEnumerator.Current
            => Current;

        public bool MoveNext()
            => ++index < array.Length;

        public readonly void Dispose()
        { }

        public void Reset()
            => throw new NotImplementedException();
    }
}
