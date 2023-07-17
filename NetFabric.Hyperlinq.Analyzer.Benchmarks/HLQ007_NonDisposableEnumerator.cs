using BenchmarkDotNet.Attributes;
using System.Collections;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq.Analyzer.Benchmarks;

public class HLQ007_NonDisposableEnumerator
{
    TestEnumerableDisposable<int>? enumerableDisposable;
    TestEnumerable<int>? enumerable;

    [Params(100_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var array = Enumerable.Range(0, 10)
            .ToArray();
        enumerableDisposable = new(array);
        enumerable = new(array);
    }

    [Benchmark(Baseline = true)]
    public int Disposable()
    {
        var sum = 0;
        for(var count = 0; count < Count; count++)
            sum += SumDisposable(enumerableDisposable!);
        return sum;
    }

    [Benchmark]
    public int NonDisposable()
    {
        var sum = 0;
        for (var count = 0; count < Count; count++)
            sum += Sum(enumerable!);
        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Sum(TestEnumerable<int> values)
    {
        var sum = 0;
        foreach (var value in values)
            sum += value;
        return sum;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int SumDisposable(TestEnumerableDisposable<int> values)
    {
        var sum = 0;
        foreach (var value in values)
            sum += value;
        return sum;
    }

    public class TestEnumerable<T>
        : IEnumerable<T>
    {
        readonly T[] array;

        public TestEnumerable(T[] array)
            => this.array = array;

        public Enumerator<T> GetEnumerator()
            => new(array);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => new DisposableEnumerator<T>();

        IEnumerator IEnumerable.GetEnumerator()
            => new DisposableEnumerator<T>();
    }

    public class TestEnumerableDisposable<T> 
        : IEnumerable<T>
    {
        readonly T[] array;

        public TestEnumerableDisposable(T[] array)
            => this.array = array;

        public DisposableEnumerator<T> GetEnumerator()
            => new(array);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public struct Enumerator<T>
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

        public bool MoveNext()
            => ++index < array.Length;
    }

    public struct DisposableEnumerator<T>
        : IEnumerator<T>
    {
        readonly T[] array;
        int index;

        public DisposableEnumerator(T[] array)
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
