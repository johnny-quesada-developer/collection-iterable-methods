#if TEST

namespace Test
{
    using System.Collections.Generic;
    using CollectionIterableAsync;
    using Xunit;
    using System.Diagnostics;

    public class CollectionIterableAsyncTest
    {
        public CollectionIterableAsyncTest()
        {
#if TEST_DEBUG

            Debugger.Launch();

            while (!Debugger.IsAttached) Thread.Sleep(500);
#endif
        }

        #region ConcatAsync

        [Fact]
        public async Task ConcatAsync()
        {
            int[] first = new[] { 1, 2, 3 };
            int[] second = new[] { 4, 5, 6 };

            var result = await first.ConcatAsync(second);

            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, result);
        }

        [Fact]
        public async Task ConcatAsyncIEnumerable()
        {
            IEnumerable<int> first = new List<int> { 1, 2, 3 };
            IEnumerable<int> second = new List<int> { 4, 5, 6 };

            var result = await first.ConcatAsync(second);

            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, result.ToArray());
        }

        [Fact]
        public async Task ConcatAsyncICollection()
        {
            ICollection<int> first = new List<int> { 1, 2, 3 };
            ICollection<int> second = new List<int> { 4, 5, 6 };

            var result = await first.ConcatAsync(second);

            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, result.ToArray());
        }

        #endregion

        #region FilterAsync

        [Fact]
        public async Task FilterAsync()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };

            var result = await source.FilterAsync(item => item % 2 == 0);

            Assert.Equal(new[] { 2, 4 }, result.ToArray());
        }

        [Fact]
        public async Task FilterAsyncIEnumerable()
        {
            IEnumerable<int> source = new List<int> { 1, 2, 3, 4, 5 };

            var result = await source.FilterAsync(item => item % 2 == 0);

            Assert.Equal(new[] { 2, 4 }, result.ToArray());
        }

        [Fact]
        public async Task FilterAsyncICollection()
        {
            ICollection<int> source = new List<int> { 1, 2, 3, 4, 5 };

            var result = await source.FilterAsync(item => item % 2 == 0);

            Assert.Equal(new[] { 2, 4 }, result.ToArray());
        }

        #endregion

        #region FilterParallelAsync

        [Fact]
        public async Task FilterParallelAsync()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };

            var result = await source.FilterParallelAsync(item => item % 2 == 0);

            Assert.Equal(2, result.Count());
            Assert.Contains(2, result);
            Assert.Contains(4, result);
        }

        [Fact]
        public async Task FilterParallelAsyncIEnumerable()
        {
            IEnumerable<int> source = new List<int> { 1, 2, 3, 4, 5 };

            var result = await source.FilterParallelAsync(item => item % 2 == 0);

            Assert.Equal(2, result.Count());
            Assert.Contains(2, result);
            Assert.Contains(4, result);
        }

        [Fact]
        public async Task FilterParallelAsyncICollection()
        {
            ICollection<int> source = new List<int> { 1, 2, 3, 4, 5 };

            var result = await source.FilterParallelAsync(item => item % 2 == 0);

            Assert.Equal(2, result.Count());
            Assert.Contains(2, result);
            Assert.Contains(4, result);
        }

        #endregion

        #region ForEachAsync

        [Fact]
        public async Task ForEachAsync()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };

            var result = new List<int>();

            await source.ForEachAsync(item => result.Add(item * 2));

            Assert.Equal(new[] { 2, 4, 6, 8, 10 }, result.ToArray());
        }

        [Fact]
        public async Task ForEachAsyncIEnumerable()
        {
            IEnumerable<int> source = new List<int> { 1, 2, 3, 4, 5 };

            var result = new List<int>();

            await source.ForEachAsync(item => result.Add(item * 2));

            Assert.Equal(new[] { 2, 4, 6, 8, 10 }, result.ToArray());
        }


        [Fact]
        public async Task ForEachAsyncICollection()
        {
            ICollection<int> source = new List<int> { 1, 2, 3, 4, 5 };

            var result = new List<int>();

            await source.ForEachAsync(item => result.Add(item * 2));

            Assert.Equal(new[] { 2, 4, 6, 8, 10 }, result.ToArray());
        }

        #endregion
    }
}
#endif
