#if TEST

namespace Test
{
  using System;
  using Xunit;
  using CollectionIterableParallel;
  using CollectionIterableUtils;
  using CollectionIterable;
  using System.Diagnostics;

  public class CollectionIterableParallelTest
  {
    public CollectionIterableParallelTest()
    {
#if TEST_DEBUG

            Debugger.Launch();

            while (!Debugger.IsAttached) Thread.Sleep(500);
#endif
    }

    #region FilterParallel

    [Fact]
    public void FilterParallel()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      var result = source.FilterParallel(item => item % 2 == 0, null).ToArray();

      Array.Sort(result);

      Assert.Equal(new[] { 2, 4 }, result);
    }

    [Fact]
    public void FilterParallelIEnumerable()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      var result = source.FilterParallel(item => item % 2 == 0, null).ToArray();

      Array.Sort(result);

      Assert.Equal(new[] { 2, 4 }, result);
    }

    [Fact]
    public void FilterParallelICollection()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      var result = source.FilterParallel(item => item % 2 == 0, null).ToArray();

      Array.Sort(result);

      Assert.Equal(new[] { 2, 4 }, result);
    }

    [Fact]
    public void FilterParallelWhenCancelled()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      var options = new IIterableOptions
      {
        cancellationToken = new System.Threading.CancellationToken(true)
      };

      var exception = Assert.Throws<AggregateException>(() => source.FilterParallel(item => item % 2 == 0, options).ToArray());

      // Check if the AggregateException contains an OperationCanceledException
      Assert.Contains(exception.InnerExceptions, ex => ex is OperationCanceledException);
    }

    #endregion

    #region ForEachParallel

    [Fact]
    public void ForEachParallel()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      var result = new List<int>();

      source.ForEachParallel(item => result.Add(item), null);

      Assert.Contains(1, result);
      Assert.Contains(2, result);
      Assert.Contains(3, result);
      Assert.Contains(4, result);
      Assert.Contains(5, result);
      Assert.Equal(5, result.Count);
    }

    [Fact]
    public void ForEachParallelIEnumerable()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      var result = new List<int>();

      source.ForEachParallel(item => result.Add(item), null);

      Assert.Contains(1, result);
      Assert.Contains(2, result);
      Assert.Contains(3, result);
      Assert.Contains(4, result);
      Assert.Contains(5, result);

      Assert.Equal(5, result.Count);
    }

    [Fact]
    public void ForEachParallelICollection()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      var result = new List<int>();

      source.ForEachParallel(item => result.Add(item), null);

      Assert.Contains(1, result);
      Assert.Contains(2, result);
      Assert.Contains(3, result);
      Assert.Contains(4, result);
      Assert.Contains(5, result);

      Assert.Equal(5, result.Count);
    }

    [Fact]
    public void ForEachParallelWhenCancelled()
    {
      var source = new[] { 1, 2, 3, 4, 5 };

      IIterableOptions options = new IIterableOptions
      {
        cancellationToken = new System.Threading.CancellationToken(true)
      };

      var exception = Assert.Throws<AggregateException>(() => source.ForEachParallel(item =>
      {
        options.cancellationToken?.ThrowIfCancellationRequested();
      }, options));

      // Check if the AggregateException contains an OperationCanceledException
      Assert.Contains(exception.InnerExceptions, ex => ex is OperationCanceledException);
    }

    #endregion

    #region SortCollectionParallel

    [Fact]
    public void SortCollectionParallelAscending()
    {
      var source = new[] { 5, 4, 3, 2, 1, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

      var result = source.SortCollectionParallel(n => n, SortDirection.Ascending);

      Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, result);
    }

    [Fact]
    public void SortCollectionParallelDescending()
    {
      var source = new[] { 5, 4, 3, 2, 1, 6, 7, 8, 9, 10 };

      var result = source.SortCollectionParallel(n => n, SortDirection.Descending);

      Assert.Equal(new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, result);
    }

    [Fact]
    public void SortCollectionParallelIEnumerableAscending()
    {
      IEnumerable<int> source = new List<int> { 5, 4, 3, 2, 1, 6, 7, 8, 9, 10 };

      var result = source.SortCollectionParallel(n => n, SortDirection.Ascending);

      Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, result.ToArray());
    }

    [Fact]
    public void SortCollectionParallelIEnumerableDescending()
    {
      IEnumerable<int> source = new List<int> { 5, 4, 3, 2, 1, 6, 7, 8, 9, 10 };

      var result = source.SortCollectionParallel(n => n, SortDirection.Descending);

      Assert.Equal(new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, result.ToArray());
    }

    #endregion

    #region ToDictionaryParallel

    [Fact]
    public void ToDictionaryParallel()
    {
      var source = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("A", "x10"),
                new KeyValuePair<string, string>("B", "x20"),
                new KeyValuePair<string, string>("C", "x30"),
                new KeyValuePair<string, string>("D", "x40"),
                new KeyValuePair<string, string>("E", "x50"),
            };

      var result =
          source.ToDictionaryParallel(
              item => new KeyValuePair<string, string>(
                  item.Key, item.Value
              ), null
          );

      Assert.Equal("x10", result["A"]);
      Assert.Equal("x20", result["B"]);
      Assert.Equal("x30", result["C"]);
      Assert.Equal("x40", result["D"]);
      Assert.Equal("x50", result["E"]);
    }

    [Fact]
    public void ToDictionaryParallelIEnumerable()
    {
      IEnumerable<KeyValuePair<string, string>> source = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("A", "x10"),
                new KeyValuePair<string, string>("B", "x20"),
                new KeyValuePair<string, string>("C", "x30"),
                new KeyValuePair<string, string>("D", "x40"),
                new KeyValuePair<string, string>("E", "x50"),
            };

      var result = source.ToDictionaryParallel(
          (item) => new KeyValuePair<string, string>(
                  item.Key, item.Value
              ), null
          );

      Assert.Equal("x10", result["A"]);
      Assert.Equal("x20", result["B"]);
      Assert.Equal("x30", result["C"]);
      Assert.Equal("x40", result["D"]);
      Assert.Equal("x50", result["E"]);
    }

    #endregion

  }
}

#endif
