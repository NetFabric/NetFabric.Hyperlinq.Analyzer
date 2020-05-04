using System;
using System.Collections.Generic;
using System.Text;

namespace HLQ005
{
    static class EnumerableExtensions
    {
        public static T First<T>(this IEnumerable<T> source)
            => default;

        public static T First<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => default;

        public static T FirstOrDefault<T>(this IEnumerable<T> source)
            => default;

        public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => default;

        public static T Single<T>(this IEnumerable<T> source)
            => default;

        public static T Single<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => default;

        public static T SingleOrDefault<T>(this IEnumerable<T> source)
            => default;

        public static T SingleOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => default;
    }
}
