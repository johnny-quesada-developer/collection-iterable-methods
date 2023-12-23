namespace CollectionIterableParallel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using CollectionIterable;
    using CollectionIterableUtils;

    public static class CollectionIterableParallel
    {
        #region FilterParallel

        internal static IEnumerable<T> FilterParallelCommon<T>(IEnumerable<T> source, Func<T, Boolean> callback, IIterableOptions? options)
        {
            if (options == null)
            {
                options = new IIterableOptions();
            }

            if (options.parallelOptions == null)
            {
                options.parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 5 };
            }

            var result = new ConcurrentBag<T>();

            Parallel.ForEach(source, options.parallelOptions, (item) =>
            {
                options.cancellationToken?.ThrowIfCancellationRequested();

                if (callback(item))
                {
                    result.Add(item);
                }
            });

            return result;
        }

        public static IEnumerable<T> FilterParallel<T>(this T[] source, Func<T, Boolean> callback, IIterableOptions? options)
        {
            return FilterParallelCommon(source, callback, options);
        }

        public static IEnumerable<T> FilterParallel<T>(this ICollection<T> source, Func<T, Boolean> callback, IIterableOptions? options)
        {
            return FilterParallelCommon(source, callback, options);
        }

        public static IEnumerable<T> FilterParallel<T>(this IEnumerable<T> source, Func<T, Boolean> callback, IIterableOptions? options)
        {
            return FilterParallelCommon(source, callback, options);
        }

        #endregion

        #region ForEachParallel

        internal static void ForEachParallelCommon<T>(IEnumerable<T> source, Action<T> callback, IIterableOptions? options)
        {
            if (options == null)
            {
                options = new IIterableOptions();
            }

            if (options.parallelOptions == null)
            {
                options.parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 5 };
            }

            Parallel.ForEach(source, options.parallelOptions, callback);
        }

        public static void ForeachParallel<T>(this T[] source, Action<T> callback, IIterableOptions? options)
        {
            ForEachParallelCommon(source, callback, options);
        }

        public static void ForEachParallel<T>(this ICollection<T> source, Action<T> callback, IIterableOptions? options)
        {
            ForEachParallelCommon(source, callback, options);
        }

        public static void ForEachParallel<T>(this IEnumerable<T> source, Action<T> callback, IIterableOptions? options)
        {
            ForEachParallelCommon(source, callback, options);
        }

        #endregion

        #region SortCollectionParallel

        internal static void QuickSortParallel<T, TKey>(T[] array, int left, int right, Func<T, TKey> keySelector, SortDirection direction) where TKey : IComparable<TKey>
        {
            if (left < right)
            {
                int pivotIndex = CollectionIterable.Partition(array, left, right, keySelector, direction);

                Parallel.Invoke(
                    () => CollectionIterable.QuickSort(array, left, pivotIndex - 1, keySelector, direction),
                    () => CollectionIterable.QuickSort(array, pivotIndex + 1, right, keySelector, direction)
                );
            }
        }

        internal static IEnumerable<T> SortCollectionParallelCommon<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            var array = source.ToArray();

            QuickSortParallel(array, 0, array.Length - 1, keySelector, direction);

            return array;
        }

        public static IEnumerable<T> SortCollectionParallel<T, TKey>(this T[] source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            return SortCollectionParallelCommon(source, keySelector, direction);
        }

        public static IEnumerable<T> SortCollectionParallel<T, TKey>(this ICollection<T> source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            return SortCollectionParallelCommon(source, keySelector, direction);
        }

        public static IEnumerable<T> SortCollectionParallel<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            return SortCollectionParallelCommon(source, keySelector, direction);
        }

        #endregion

        #region ToDictionaryParallel 

        internal static IDictionary<TKey, TValue> ToDictionaryParallelCommon<T, TKey, TValue>(IEnumerable<T> source, Func<T, KeyValuePair<TKey, TValue>> callback, IIterableOptions? options) where TKey : notnull
        {
            if (options == null)
            {
                options = new IIterableOptions();
            }

            if (options.parallelOptions == null)
            {
                options.parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 5 };
            }

            var dictionary = new ConcurrentDictionary<TKey, TValue>();

            Parallel.ForEach(source, options.parallelOptions, (item, state) =>
            {
                options.cancellationToken?.ThrowIfCancellationRequested();

                var pair = callback(item);

                dictionary.TryAdd(pair.Key, pair.Value);
            });

            return dictionary;
        }

        public static IDictionary<TKey, TValue> ToDictionaryParallel<T, TKey, TValue>(this T[] source, Func<T, KeyValuePair<TKey, TValue>> callback, IIterableOptions? options = null) where TKey : notnull
        {
            return ToDictionaryParallelCommon(source, callback, options);
        }

        public static IDictionary<TKey, TValue> ToDictionaryParallel<T, TKey, TValue>(this ICollection<T> source, Func<T, KeyValuePair<TKey, TValue>> callback, IIterableOptions? options = null) where TKey : notnull
        {
            return ToDictionaryParallelCommon(source, callback, options);
        }

        public static IDictionary<TKey, TValue> ToDictionaryParallel<T, TKey, TValue>(this IEnumerable<T> source, Func<T, KeyValuePair<TKey, TValue>> callback, IIterableOptions? options = null) where TKey : notnull
        {
            return ToDictionaryParallelCommon(source, callback, options);
        }

        #endregion

    }
}