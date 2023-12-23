namespace CollectionIterable
{
    using System;
    using System.Collections.Generic;
    using CollectionIterableUtils;

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public static class CollectionIterable
    {
        #region Concat

        internal static IEnumerable<T> ConcatEnumerables<T>(IEnumerable<T> first, IEnumerable<T> second, IIterableOptions? options)
        {
            foreach (var item in first)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                yield return item;
            }

            foreach (var item in second)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                yield return item;
            }
        }

        internal static IEnumerable<T> ConcatCommon<T>(IEnumerable<T> first, IEnumerable<T> second, IIterableOptions? options)
        {
            // Optimize for arrays
            if (first is T[] array1 && second is T[] array2)
            {
                T[] result = new T[array1.Length + array2.Length];

                Array.Copy(array1, result, array1.Length);
                Array.Copy(array2, 0, result, array1.Length, array2.Length);

                return result;
            }

            // Optimize for ICollection<T>
            if (first is ICollection<T> collection1 && second is ICollection<T> collection2)
            {
                var resultList = new List<T>(collection1.Count + collection2.Count);

                resultList.AddRange(collection1);
                resultList.AddRange(collection2);

                return resultList;
            }

            // Default to enumerable concatenation
            return ConcatEnumerables(first, second, options);
        }

        public static IEnumerable<T> Concat<T>(this T[] first, T[] second)
        {
            return ConcatCommon(first: first, second, null);
        }

        public static IEnumerable<T> Concat<T>(this ICollection<T> first, T[] second)
        {
            return ConcatCommon(first: first, second, null);
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, T[] second)
        {
            return ConcatCommon(first: first, second, null);
        }

        #endregion

        #region Filter

        internal static IEnumerable<T> FilterCommon<T>(IEnumerable<T> source, Func<T, Int32, Boolean> callback, IIterableOptions? options)
        {
            var index = 0;

            foreach (var item in source)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                if (callback(item, index))
                {
                    yield return item;
                }

                index++;
            }
        }

        public static IEnumerable<T> Filter<T>(this T[] source, Func<T, Int32, Boolean> callback)
        {
            return FilterCommon(source, callback, null);
        }

        public static IEnumerable<T> Filter<T>(this T[] source, Func<T, Boolean> callback)
        {
            return FilterCommon(source, (item, index) => callback(item), null);
        }

        public static IEnumerable<T> Filter<T>(this ICollection<T> source, Func<T, Int32, Boolean> callback)
        {
            return FilterCommon(source, callback, null);
        }

        public static IEnumerable<T> Filter<T>(this ICollection<T> source, Func<T, Boolean> callback)
        {
            return FilterCommon(source, (item, index) => callback(item), null);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, Int32, Boolean> callback)
        {
            return FilterCommon(source, callback, null);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, Boolean> callback)
        {
            return FilterCommon(source, (item, index) => callback(item), null);
        }

        #endregion

        #region ForEach

        internal static void ForEachCommon<T>(IEnumerable<T> source, Action<T, Int32> callback, IIterableOptions? options)
        {
            var index = 0;

            foreach (var item in source)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                callback(item, index);

                index++;
            }
        }

        /**
        * @description: Executes a provided function once for each array element.
        * @callback: item, index
        * @return: void
        */
        public static void ForEach<T>(this T[] source, Action<T, Int32> callback)
        {
            ForEachCommon(source, callback, null);
        }

        public static void ForEach<T>(this T[] source, Action<T> callback)
        {
            ForEachCommon(source, (item, index) => callback(item), null);
        }

        public static void ForEach<T>(this ICollection<T> source, Action<T, Int32> callback)
        {
            ForEachCommon(source, callback, null);
        }

        public static void ForEach<T>(this ICollection<T> source, Action<T> callback)
        {
            ForEachCommon(source, (item, index) => callback(item), null);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, Int32> callback)
        {
            ForEachCommon(source, callback, null);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> callback)
        {
            ForEachCommon(source, (item, index) => callback(item), null);
        }

        #endregion

        #region Map

        internal static IEnumerable<T> MapCommon<T>(IEnumerable<T> source, Func<T, Int32, T> callback, IIterableOptions? options)
        {
            var index = 0;

            foreach (var item in source)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                yield return callback(item, index);

                index++;
            }
        }

        public static IEnumerable<T> Map<T>(this T[] source, Func<T, Int32, T> callback) where T : IEnumerable<T>, new()
        {
            return MapCommon(source, callback, null);
        }

        public static IEnumerable<T> Map<T>(this T[] source, Func<T, T> callback) where T : IEnumerable<T>, new()
        {
            return MapCommon(source, (item, index) => callback(item), null);
        }

        public static IEnumerable<T> Map<T>(this ICollection<T> source, Func<T, Int32, T> callback)
        {
            return MapCommon(source, callback, null);
        }

        public static IEnumerable<T> Map<T>(this ICollection<T> source, Func<T, T> callback)
        {
            return MapCommon(source, (item, index) => callback(item), null);
        }

        public static IEnumerable<T> Map<T>(this IEnumerable<T> source, Func<T, Int32, T> callback)
        {
            return MapCommon(source, callback, null);
        }

        public static IEnumerable<T> Map<T>(this IEnumerable<T> source, Func<T, T> callback)
        {
            return MapCommon(source, (item, index) => callback(item), null);
        }

        #endregion

        #region Reduce

        internal static IResult ReduceCommon<T, IResult>(IEnumerable<T> source, Func<IResult, T, Int32, IResult> callback, IResult initialValue, IIterableOptions? options)
        {
            IResult accumulator = initialValue;

            var index = 0;

            foreach (var item in source)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                accumulator = callback(accumulator, item, index);

                index++;
            }

            return accumulator;
        }

        /**
        *
        */
        public static IResult Reduce<T, IResult>(this T[] source, Func<IResult, T, Int32, IResult> callback, IResult initialValue)
        {
            return ReduceCommon(source, callback, initialValue, null);
        }

        public static IResult Reduce<T, IResult>(this T[] source, Func<IResult, T, IResult> callback, IResult initialValue)
        {
            return ReduceCommon(source, (accumulator, item, index) => callback(accumulator, item), initialValue, null);
        }

        public static IResult Reduce<T, IResult>(this ICollection<T> source, Func<IResult, T, Int32, IResult> callback, IResult initialValue)
        {
            return ReduceCommon(source, callback, initialValue, null);
        }

        public static IResult Reduce<T, IResult>(this ICollection<T> source, Func<IResult, T, IResult> callback, IResult initialValue)
        {
            return ReduceCommon(source, (accumulator, item, index) => callback(accumulator, item), initialValue, null);
        }

        public static IResult Reduce<T, IResult>(this IEnumerable<T> source, Func<IResult, T, Int32, IResult> callback, IResult initialValue)
        {
            return ReduceCommon(source, callback, initialValue, null);
        }

        public static IResult Reduce<T, IResult>(this IEnumerable<T> source, Func<IResult, T, IResult> callback, IResult initialValue)
        {
            return ReduceCommon(source, (accumulator, item, index) => callback(accumulator, item), initialValue, null);
        }

        #endregion

        #region Slice

        internal static IEnumerable<T> SliceCommon<T>(IEnumerable<T> source, int start, int end, IIterableOptions? options)
        {
            var sourceLength = 0;

            // if the source is an array, we can use the length property
            if (source is T[] array)
            {
                sourceLength = array.Length;

                for (var index = start; index < end && index < sourceLength; index++)
                {
                    options?.cancellationToken?.ThrowIfCancellationRequested();

                    yield return array[index];
                }

                yield break; // Exit the method after handling the array
            }

            if (source is ICollection<T> collection)
            {
                sourceLength = collection.Count;
            }

            else if (source is IEnumerable<T> enumerable)
            {
                sourceLength = enumerable.Count();
            }

            for (var index = start; index < end && index < sourceLength; index++)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                yield return source.ElementAt(index);
            }
        }

        public static IEnumerable<T> Slice<T>(this T[] source, int start, int end)
        {
            return SliceCommon(source, start, end, null);
        }

        public static IEnumerable<T> Slice<T>(this ICollection<T> source, int start, int end)
        {
            return SliceCommon(source, start, end, null);
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int start, int end)
        {
            return SliceCommon(source, start, end, null);
        }

        #endregion

        #region ToRecord

        internal static IDictionary<TKey, TValue> ToRecordCommon<T, TKey, TValue>(IEnumerable<T> source, Func<T, int, KeyValuePair<TKey, TValue>> callback, IIterableOptions? options) where TKey : notnull
        {
            var dictionary = new Dictionary<TKey, TValue>();

            var index = 0;

            foreach (var item in source)
            {
                options?.cancellationToken?.ThrowIfCancellationRequested();

                var keyValuePair = callback(item, index);

                dictionary.Add(keyValuePair.Key, keyValuePair.Value);

                index++;
            }

            return dictionary;
        }

        public static IDictionary<TKey, TValue> ToRecord<T, TKey, TValue>(this T[] source, Func<T, int, KeyValuePair<TKey, TValue>> callback) where TKey : notnull
        {
            return ToRecordCommon(source, callback, null);
        }

        public static IDictionary<TKey, TValue> ToRecord<T, TKey, TValue>(this T[] source, Func<T, KeyValuePair<TKey, TValue>> callback) where TKey : notnull
        {
            return ToRecordCommon(source, (item, index) => callback(item), null);
        }

        public static IDictionary<TKey, TValue> ToRecord<T, TKey, TValue>(this ICollection<T> source, Func<T, int, KeyValuePair<TKey, TValue>> callback) where TKey : notnull
        {
            return ToRecordCommon(source, callback, null);
        }

        public static IDictionary<TKey, TValue> ToRecord<T, TKey, TValue>(this ICollection<T> source, Func<T, KeyValuePair<TKey, TValue>> callback) where TKey : notnull
        {
            return ToRecordCommon(source, (item, index) => callback(item), null);
        }

        public static IDictionary<TKey, TValue> ToRecord<T, TKey, TValue>(this IEnumerable<T> source, Func<T, int, KeyValuePair<TKey, TValue>> callback) where TKey : notnull
        {
            return ToRecordCommon(source, callback, null);
        }

        public static IDictionary<TKey, TValue> ToRecord<T, TKey, TValue>(this IEnumerable<T> source, Func<T, KeyValuePair<TKey, TValue>> callback) where TKey : notnull
        {
            return ToRecordCommon(source, (item, index) => callback(item), null);
        }

        #endregion

        #region SortCollection

        internal static void QuickSort<T, TKey>(T[] array, int left, int right, Func<T, TKey> keySelector, SortDirection direction) where TKey : IComparable<TKey>
        {
            if (left < right)
            {
                int pivotIndex = Partition(array, left, right, keySelector, direction);

                QuickSort(array, left, pivotIndex - 1, keySelector, direction);
                QuickSort(array, pivotIndex + 1, right, keySelector, direction);
            }
        }

        internal static int Partition<T, TKey>(T[] array, int left, int right, Func<T, TKey> keySelector, SortDirection direction) where TKey : IComparable<TKey>
        {
            var pivot = keySelector(array[right]);
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                bool shouldSwap = direction == SortDirection.Ascending
                    ? keySelector(array[j]).CompareTo(pivot) < 0
                    : keySelector(array[j]).CompareTo(pivot) > 0;

                if (shouldSwap)
                {
                    i++;
                    Swap(ref array[i], ref array[j]);
                }
            }

            Swap(ref array[i + 1], ref array[right]);
            return i + 1;
        }

        private static void Swap<T>(ref T x, ref T y)
        {
            T temp = x;
            x = y;
            y = temp;
        }

        internal static IEnumerable<T> SortCollectionCommon<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            var array = source.ToArray();

            QuickSort(array, 0, array.Length - 1, keySelector, direction);

            return array;
        }

        public static IEnumerable<T> SortCollection<T, TKey>(this T[] source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            return SortCollectionCommon(source, keySelector, direction);
        }

        public static IEnumerable<T> SortCollection<T, TKey>(this ICollection<T> source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            return SortCollectionCommon(source, keySelector, direction);
        }

        public static IEnumerable<T> SortCollection<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, SortDirection direction = SortDirection.Ascending) where TKey : IComparable<TKey>
        {
            return SortCollectionCommon(source, keySelector, direction);
        }

        #endregion
    }
}