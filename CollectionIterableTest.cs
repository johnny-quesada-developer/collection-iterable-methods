#if TEST

namespace Test
{
    using Xunit;
    using CollectionIterable;
    using System.Diagnostics;

    // ok, so .test is not being included, but this name is included... so we can review there
    public class CollectionIterableTest
    {
        public CollectionIterableTest()
        {
#if TEST_DEBUG

            Debugger.Launch();

            while (!Debugger.IsAttached) Thread.Sleep(500);
#endif
        }

        #region Concat

        [Fact]
        public void Concat()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 4, 5, 6 };

            var result = array1.Concat(array2);

            Assert.Equal(6, result.Count());
            Assert.Equal(1, result.ElementAt(0));
            Assert.Equal(2, result.ElementAt(1));
            Assert.Equal(3, result.ElementAt(2));
            Assert.Equal(4, result.ElementAt(3));
            Assert.Equal(5, result.ElementAt(4));
            Assert.Equal(6, result.ElementAt(5));
        }

        [Fact]
        public void ConcatCollection()
        {
            ICollection<int> collection1 = new List<int>() { 1, 2, 3 };
            ICollection<int> collection2 = new List<int>() { 4, 5, 6 };

            var result = collection1.Concat(collection2);

            Assert.Equal(6, result.Count());
            Assert.Equal(1, result.ElementAt(0));
            Assert.Equal(2, result.ElementAt(1));
            Assert.Equal(3, result.ElementAt(2));
            Assert.Equal(4, result.ElementAt(3));
            Assert.Equal(5, result.ElementAt(4));
            Assert.Equal(6, result.ElementAt(5));
        }

        [Fact]
        public void ConcatEnumerable()
        {
            IEnumerable<int> enumerable1 = new List<int>() { 1, 2, 3 };
            IEnumerable<int> enumerable2 = new List<int>() { 4, 5, 6 };

            var result = enumerable1.Concat(enumerable2);

            Assert.Equal(6, result.Count());
            Assert.Equal(1, result.ElementAt(0));
            Assert.Equal(2, result.ElementAt(1));
            Assert.Equal(3, result.ElementAt(2));
            Assert.Equal(4, result.ElementAt(3));
            Assert.Equal(5, result.ElementAt(4));
            Assert.Equal(6, result.ElementAt(5));
        }

        #endregion

        #region Filter

        [Fact]
        public void Filter()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6 };

            var result = array.Filter(item => item % 2 == 0);

            Assert.Equal(3, result.Count());
            Assert.Equal(2, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
            Assert.Equal(6, result.ElementAt(2));
        }

        [Fact]
        public void FilterCollection()
        {
            ICollection<int> collection = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = collection.Filter(item => item % 2 == 0);

            Assert.Equal(3, result.Count());
            Assert.Equal(2, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
            Assert.Equal(6, result.ElementAt(2));
        }

        [Fact]
        public void FilterEnumerable()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = enumerable.Filter(item => item % 2 == 0);

            Assert.Equal(3, result.Count());
            Assert.Equal(2, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
            Assert.Equal(6, result.ElementAt(2));
        }

        #endregion

        #region ForEach

        [Fact]
        public void ForEach()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6 };

            var result = new List<int>();

            array.ForEach(item => result.Add(item));

            Assert.Equal(6, result.Count());
            Assert.Equal(1, result.ElementAt(0));
            Assert.Equal(2, result.ElementAt(1));
            Assert.Equal(3, result.ElementAt(2));
            Assert.Equal(4, result.ElementAt(3));
            Assert.Equal(5, result.ElementAt(4));
            Assert.Equal(6, result.ElementAt(5));
        }

        [Fact]
        public void ForEachCollection()
        {
            ICollection<int> collection = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = new List<int>();

            collection.ForEach(item => result.Add(item));

            Assert.Equal(6, result.Count());
            Assert.Equal(1, result.ElementAt(0));
            Assert.Equal(2, result.ElementAt(1));
            Assert.Equal(3, result.ElementAt(2));
            Assert.Equal(4, result.ElementAt(3));
            Assert.Equal(5, result.ElementAt(4));
            Assert.Equal(6, result.ElementAt(5));
        }

        [Fact]
        public void ForEachEnumerable()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = new List<int>();

            enumerable.ForEach(item => result.Add(item));

            Assert.Equal(6, result.Count());
            Assert.Equal(1, result.ElementAt(0));
            Assert.Equal(2, result.ElementAt(1));
            Assert.Equal(3, result.ElementAt(2));
            Assert.Equal(4, result.ElementAt(3));
            Assert.Equal(5, result.ElementAt(4));
            Assert.Equal(6, result.ElementAt(5));
        }

        #endregion

        #region Map

        [Fact]
        public void Map()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6 };

            var result = array.Map(item => item * 2);

            Assert.Equal(6, result.Count());
            Assert.Equal(2, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
            Assert.Equal(6, result.ElementAt(2));
            Assert.Equal(8, result.ElementAt(3));
            Assert.Equal(10, result.ElementAt(4));
            Assert.Equal(12, result.ElementAt(5));
        }

        [Fact]
        public void MapCollection()
        {
            ICollection<int> collection = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = collection.Map(item => item * 2);

            Assert.Equal(6, result.Count());
            Assert.Equal(2, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
            Assert.Equal(6, result.ElementAt(2));
            Assert.Equal(8, result.ElementAt(3));
            Assert.Equal(10, result.ElementAt(4));
            Assert.Equal(12, result.ElementAt(5));
        }

        [Fact]
        public void MapEnumerable()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = enumerable.Map(item => item * 2);

            Assert.Equal(6, result.Count());
            Assert.Equal(2, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
            Assert.Equal(6, result.ElementAt(2));
            Assert.Equal(8, result.ElementAt(3));
            Assert.Equal(10, result.ElementAt(4));
            Assert.Equal(12, result.ElementAt(5));
        }

        #endregion

        #region Reduce

        [Fact]
        public void Reduce()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6 };

            var result = array.Reduce((acc, item) => acc + item, 0);

            Assert.Equal(21, result);
        }

        [Fact]
        public void ReduceCollection()
        {
            ICollection<int> collection = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = collection.Reduce((acc, item) => acc + item, 0);

            Assert.Equal(21, result);
        }

        [Fact]
        public void ReduceEnumerable()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = enumerable.Reduce((acc, item) => acc + item, 0);

            Assert.Equal(21, result);
        }

        #endregion

        #region Slice

        [Fact]
        public void Slice()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6 };

            var result = array.Slice(2, 4);

            Assert.Equal(2, result.Count());
            Assert.Equal(3, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
        }

        [Fact]
        public void SliceCollection()
        {
            ICollection<int> collection = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = collection.Slice(2, 4);

            Assert.Equal(2, result.Count());
            Assert.Equal(3, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
        }

        [Fact]
        public void SliceEnumerable()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = enumerable.Slice(2, 4);

            Assert.Equal(2, result.Count());
            Assert.Equal(3, result.ElementAt(0));
            Assert.Equal(4, result.ElementAt(1));
        }

        #endregion

        #region ToRecord

        [Fact]
        public void ToRecord()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6 };

            var result = array.ToRecord(item => new KeyValuePair<int, int>(item, item * 2));

            Assert.Equal(6, result.Count());
            Assert.Equal(2, result[1]);
            Assert.Equal(4, result[2]);
            Assert.Equal(6, result[3]);
            Assert.Equal(8, result[4]);
            Assert.Equal(10, result[5]);
            Assert.Equal(12, result[6]);
        }

        [Fact]
        public void ToRecordCollection()
        {
            ICollection<int> collection = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = collection.ToRecord(item => new KeyValuePair<int, int>(item, item * 2));

            Assert.Equal(6, result.Count());
            Assert.Equal(2, result[1]);
            Assert.Equal(4, result[2]);
            Assert.Equal(6, result[3]);
            Assert.Equal(8, result[4]);
            Assert.Equal(10, result[5]);
            Assert.Equal(12, result[6]);
        }

        [Fact]
        public void ToRecordEnumerable()
        {
            IEnumerable<int> enumerable = new List<int>() { 1, 2, 3, 4, 5, 6 };

            var result = enumerable.ToRecord(item => new KeyValuePair<int, int>(item, item * 2));

            Assert.Equal(6, result.Count());
            Assert.Equal(2, result[1]);
            Assert.Equal(4, result[2]);
            Assert.Equal(6, result[3]);
            Assert.Equal(8, result[4]);
            Assert.Equal(10, result[5]);
            Assert.Equal(12, result[6]);
        }

        [Fact]
        public void ToRecordStringAndNumbers()
        {
            var array = new[] { 1, 2, 3, 4, 5, 6 };

            var result = array.ToRecord(item => new KeyValuePair<string, int>(item.ToString(), item * 2));

            Assert.Equal(6, result.Count());
            Assert.Equal(2, result["1"]);
            Assert.Equal(4, result["2"]);
            Assert.Equal(6, result["3"]);
            Assert.Equal(8, result["4"]);
            Assert.Equal(10, result["5"]);
            Assert.Equal(12, result["6"]);
        }

        #endregion

        #region SortCollection

        [Fact]
        public void SortCollectionAscending()
        {
            var array = new[] { 3, 2, 1, 6, 5, 4 };

            var result = array.SortCollection(n => n, SortDirection.Ascending);

            Assert.Equal(6, result.Count());
            Assert.Equal(1, result.ElementAt(0));
            Assert.Equal(2, result.ElementAt(1));
            Assert.Equal(3, result.ElementAt(2));
            Assert.Equal(4, result.ElementAt(3));
            Assert.Equal(5, result.ElementAt(4));
            Assert.Equal(6, result.ElementAt(5));
        }

        [Fact]
        public void SortCollectionDescending()
        {
            var array = new[] { 3, 2, 1, 6, 5, 4 };

            var result = array.SortCollection(n => n, SortDirection.Descending);

            Assert.Equal(6, result.Count());
            Assert.Equal(6, result.ElementAt(0));
            Assert.Equal(5, result.ElementAt(1));
            Assert.Equal(4, result.ElementAt(2));
            Assert.Equal(3, result.ElementAt(3));
            Assert.Equal(2, result.ElementAt(4));
            Assert.Equal(1, result.ElementAt(5));
        }

        #endregion
    }
}

#endif
