namespace PreviousCollectionIterableAsync
{
    using System;
    using System.Collections.Generic;
    using PreviousCollectionIterable;
    using PreviousCollectionIterableParallel;
    using PreviousCollectionIterableUtils;

    public static class PreviousCollectionIterableAsync
    {
        #region Concat

        internal static Task<IEnumerable<T>> ConcatCommonAsync<T>(IEnumerable<T> first, IEnumerable<T> second, IIterableOptions? options = null)
        {
            return Task.Run(() => PreviousCollectionIterable.ConcatCommon(first, second, options));
        }

        public static Task<IEnumerable<T>> ConcatAsync<T>(this T[] first, T[] second, IIterableOptions? options = null)
        {
            return ConcatCommonAsync(first: first, second, options);
        }

        public static Task<IEnumerable<T>> ConcatAsync<T>(this ICollection<T> first, ICollection<T> second, IIterableOptions? options = null)
        {
            return ConcatCommonAsync(first: first, second, options);
        }

        public static Task<IEnumerable<T>> ConcatAsync<T>(this IEnumerable<T> first, IEnumerable<T> second, IIterableOptions? options = null)
        {
            return ConcatCommonAsync(first: first, second, options);
        }

        #endregion

        #region Filter

        internal static Task<IEnumerable<T>> FilterCommonAsync<T>(IEnumerable<T> source, Func<T, Int32, bool> callback, IIterableOptions? options)
        {
            return Task.Run(() => PreviousCollectionIterable.FilterCommon(source, callback, options));
        }

        public static Task<IEnumerable<T>> FilterAsync<T>(this T[] source, Func<T, Int32, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterCommonAsync(source, callback, options);
        }

        public static Task<IEnumerable<T>> FilterAsync<T>(this T[] source, Func<T, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterCommonAsync(source, (item, index) => callback(item), options);
        }

        public static Task<IEnumerable<T>> FilterAsync<T>(this ICollection<T> source, Func<T, Int32, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterCommonAsync(source, callback, options);
        }

        public static Task<IEnumerable<T>> FilterAsync<T>(this ICollection<T> source, Func<T, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterCommonAsync(source, (item, index) => callback(item), options);
        }

        public static Task<IEnumerable<T>> FilterAsync<T>(this IEnumerable<T> source, Func<T, Int32, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterCommonAsync(source, callback, options);
        }

        public static Task<IEnumerable<T>> FilterAsync<T>(this IEnumerable<T> source, Func<T, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterCommonAsync(source, (item, index) => callback(item), options);
        }

        #endregion

        #region FilterParallel

        internal static Task<IEnumerable<T>> FilterParallelCommonAsync<T>(IEnumerable<T> source, Func<T, Boolean> callback, IIterableOptions? options = null)
        {
            return Task.Run(() => PreviousCollectionIterableParallel.FilterParallelCommon(source, callback, options));
        }

        public static Task<IEnumerable<T>> FilterParallelAsync<T>(this T[] source, Func<T, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterParallelCommonAsync(source, callback, options);
        }

        public static Task<IEnumerable<T>> FilterParallelAsync<T>(this ICollection<T> source, Func<T, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterParallelCommonAsync(source, callback, options);
        }

        public static Task<IEnumerable<T>> FilterParallelAsync<T>(this IEnumerable<T> source, Func<T, Boolean> callback, IIterableOptions? options = null)
        {
            return FilterParallelCommonAsync(source, callback, options);
        }

        #endregion

        #region ForEachAsync

        internal static Task ForEachCommonAsync<T>(IEnumerable<T> source, Action<T, Int32> callback, IIterableOptions? options)
        {
            return Task.Run(() => PreviousCollectionIterable.ForEachCommon(source, callback, options));
        }

        public static Task ForEachAsync<T>(this T[] source, Action<T, Int32> callback, IIterableOptions? options = null)
        {
            return ForEachCommonAsync(source, callback, options);
        }

        public static Task ForEachAsync<T>(this T[] source, Action<T> callback, IIterableOptions? options = null)
        {
            return ForEachCommonAsync(source, (item, index) => callback(item), options);
        }

        public static Task ForEachAsync<T>(this ICollection<T> source, Action<T, Int32> callback, IIterableOptions? options = null)
        {
            return ForEachCommonAsync(source, callback, options);
        }

        public static Task ForEachAsync<T>(this ICollection<T> source, Action<T> callback, IIterableOptions? options = null)
        {
            return ForEachCommonAsync(source, (item, index) => callback(item), options);
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> source, Action<T, Int32> callback, IIterableOptions? options = null)
        {
            return ForEachCommonAsync(source, callback, options);
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> source, Action<T> callback, IIterableOptions? options = null)
        {
            return ForEachCommonAsync(source, (item, index) => callback(item), options);
        }

        #endregion

        #region ForeachParallel

        internal static void ForeachParallelCommonAsync<T>(IEnumerable<T> source, Action<T> callback, IIterableOptions? options = null)
        {
            Task.Run(() => PreviousCollectionIterableParallel.ForEachParallelCommon(source, callback, options));
        }

        public static void ForeachParallel<T>(this T[] source, Action<T> callback, IIterableOptions? options = null)
        {
            ForeachParallelCommonAsync(source, callback, options);
        }

        public static void ForeachParallel<T>(this ICollection<T> source, Action<T> callback, IIterableOptions? options = null)
        {
            ForeachParallelCommonAsync(source, callback, options);
        }

        public static void ForeachParallel<T>(this IEnumerable<T> source, Action<T> callback, IIterableOptions? options = null)
        {
            ForeachParallelCommonAsync(source, callback, options);
        }

        #endregion
    }
}