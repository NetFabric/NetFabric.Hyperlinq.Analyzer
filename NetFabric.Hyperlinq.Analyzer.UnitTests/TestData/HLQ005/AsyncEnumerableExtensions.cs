using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLQ005
{
    static class AsyncEnumerableExtensions
    {
        public static ValueTask<T> FirstAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
            => default;

        public static ValueTask<T> FirstAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default)
            => default;

        public static ValueTask<T> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
            => default;

        public static ValueTask<T> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default)
            => default;

        public static ValueTask<T> SingleAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
            => default;

        public static ValueTask<T> SingleAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default)
            => default;

        public static ValueTask<T> SingleOrDefaultAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
            => default;

        public static ValueTask<T> SingleOrDefaultAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default)
            => default;
    }
}
