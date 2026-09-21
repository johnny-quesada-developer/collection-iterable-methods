using Xunit;
using CollectionIterable;
using CollectionIterableAsync;
using CollectionIterableUtils;
using ParallelMethods = CollectionIterableParallel.CollectionIterableParallel;

public sealed class BehaviorTests
{
    public static IEnumerable<object[]> Shapes()
    {
        foreach (string shape in new[] { "array", "list", "iterator" })
        foreach (int count in new[] { 0, 1, 32, 1000 }) yield return new object[] { shape, count };
    }
    private static IEnumerable<int> Source(string shape, int count)
    {
        var input = Enumerable.Range(0, count).ToArray();
        return shape switch { "array" => input, "list" => input.ToList(), _ => Iterate() };
        IEnumerable<int> Iterate() { foreach (int value in input) yield return value; }
    }

    [Theory, MemberData(nameof(Shapes))]
    public void SequentialOperationsMatchReference(string shape, int count)
    {
        var source = Source(shape, count);
        Assert.Equal(source.Where(x => x % 2 == 0), source.Filter(x => x % 2 == 0));
        Assert.Equal(source.Where((x,i) => i % 2 == 0), source.Filter((x,i) => i % 2 == 0));
        Assert.Equal(source.Select(x => x+1), source.Map(x => x+1));
        Assert.Equal(source.Select((x,i) => x+i), source.Map((x,i) => x+i));
        Assert.Equal(source.Select(x => x.ToString()), source.Map(x => x.ToString()));
        Assert.Equal(source.Select((x,i) => $"{i}:{x}"), source.Map((x,i) => $"{i}:{x}"));
        Assert.Equal(source.Sum(x => (long)x), source.Reduce((sum,x) => sum+x,0L));
        Assert.Equal(source.Sum(x => (long)x)*2, source.Reduce((sum,x,i) => sum+x+i,0L));
        var visited = new List<int>();source.ForEach(x => visited.Add(x));Assert.Equal(source,visited);
        visited.Clear();source.ForEach((x,i) => visited.Add(x+i));Assert.Equal(source.Select(x=>x*2),visited);
        Assert.Equal(Enumerable.Concat(source,new[]{-1}),CollectionIterable.CollectionIterable.Concat(source,new[]{-1}));
        var record=source.ToRecord(x=>new KeyValuePair<int,int>(x,x+1));Assert.Equal(count,record.Count);
        foreach(int value in source)Assert.Equal(value+1,record[value]);
        var indexed=source.ToRecord((x,i)=>new KeyValuePair<int,int>(i,x));Assert.Equal(count,indexed.Count);
        Assert.Equal(source.Skip(count/2).Take(10),source.Slice(count/2,count/2+10));
    }

    [Fact]
    public void MapInferenceWorksForArrayAndCollection()
    {
        var array=new[]{1,2};ICollection<int> list=array.ToList();
        Assert.Equal(new[]{2,3},array.Map(x=>x+1));
        Assert.Equal(new[]{"1","2"},array.Map(x=>x.ToString()));
        Assert.Equal(new[]{"0:1","1:2"},list.Map((x,i)=>$"{i}:{x}"));
        Assert.Equal(new[]{"1","2"},array.Map<int,string>(x=>x.ToString()));
    }

    [Fact]
    public void LazyOperationsRemainLazyAndReplayable()
    {
        int calls=0;var source=new[]{1,2,3};
        var filtered=source.Filter(x=>{calls++;return x>1;});Assert.Equal(0,calls);
        Assert.Equal(new[]{2,3},filtered);Assert.Equal(3,calls);
        Assert.Equal(new[]{2,3},filtered);Assert.Equal(6,calls);
        calls=0;var mapped=source.Map(x=>{calls++;return x.ToString();});Assert.Equal(0,calls);
        Assert.Equal(new[]{"1","2","3"},mapped);Assert.Equal(3,calls);
    }

    [Fact]
    public void SliceEnumeratesOnceOnlyThroughEndAndDisposes()
    {
        int enumerations=0, visits=0;bool disposed=false;
        IEnumerable<int> Stream()
        {
            if(++enumerations>1)throw new Exception("Repeated enumeration");
            try { for(int i=0;i<100;i++){visits++;yield return i;} }
            finally { disposed=true; }
        }
        Assert.Equal(new[]{2,3,4},Stream().Slice(2,5));
        Assert.Equal(1,enumerations);Assert.Equal(5,visits);Assert.True(disposed);
        Assert.Empty(Stream().Slice(5,5));Assert.Equal(1,enumerations);
        Assert.Throws<ArgumentOutOfRangeException>(()=>Stream().Slice(-1,3).ToArray());
    }

    [Fact]
    public void RecordRejectsDuplicatesAndCallbacksRunOnce()
    {
        int calls=0;
        Assert.Throws<ArgumentException>(()=>new[]{1,1}.ToRecord(x=>{calls++;return new KeyValuePair<int,int>(x,x);}));
        Assert.Equal(2,calls);
    }

    [Theory, MemberData(nameof(Shapes))]
    public async Task AsyncOperationsFinishAtAwait(string shape,int count)
    {
        var source=Source(shape,count);int calls=0;
        var filtered=await source.FilterAsync(x=>{calls++;return x%2==0;});Assert.Equal(count,calls);
        Assert.Equal(source.Where(x=>x%2==0),filtered);Assert.Equal(source.Where(x=>x%2==0),filtered);Assert.Equal(count,calls);
        calls=0;var indexed=await source.FilterAsync((x,i)=>{calls++;return i%2==0;});Assert.Equal(count,calls);Assert.Equal(filtered,indexed);
        var concatenated=await source.ConcatAsync(source);Assert.Equal(source.Count()*2,concatenated.Count());
        var visited=new List<int>();await source.ForEachAsync(x=>visited.Add(x));Assert.Equal(source,visited);
        visited.Clear();await source.ForEachAsync((x,i)=>visited.Add(x+i));Assert.Equal(source.Select(x=>x*2),visited);
    }

    [Fact]
    public async Task AsyncExceptionsAndCancellationAreObservable()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async()=>await new[]{1}.FilterAsync((int x)=>throw new InvalidOperationException()));
        IEnumerable<int> Broken(){yield return 1;throw new InvalidOperationException();}
        await Assert.ThrowsAsync<InvalidOperationException>(async()=>await Broken().ConcatAsync(new[]{2}));
        using var cancel=new CancellationTokenSource();cancel.Cancel();var options=new IIterableOptions{cancellationToken=cancel.Token};
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>await Array.Empty<int>().FilterAsync(x=>true,options));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>await Array.Empty<int>().FilterAsync((x,i)=>true,options));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>await new[]{1}.ConcatAsync(new[]{2},options));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>await Array.Empty<int>().ForEachAsync(x=>{},options));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>await Array.Empty<int>().ForEachParallelAsync(x=>{},options));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>await Array.Empty<int>().FilterParallelAsync(x=>true,options));
    }

    [Fact]
    public async Task CancellationDuringFinalCallbackCancelsTask()
    {
        using var cancel=new CancellationTokenSource();var options=new IIterableOptions{cancellationToken=cancel.Token};
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async()=>await new[]{1}.FilterAsync(x=>{cancel.Cancel();return true;},options));
    }

    [Theory, MemberData(nameof(Shapes))]
    public async Task ParallelOperationsPreserveMembership(string shape,int count)
    {
        var source=Source(shape,count);
        var filtered=ParallelMethods.FilterParallel(source,x=>x%2==0,null);
        Assert.Equal(source.Where(x=>x%2==0),filtered.Order());
        Assert.Equal(source.Where(x=>x%2==0),(await source.FilterParallelAsync(x=>x%2==0)).Order());
        long sum=0;ParallelMethods.ForEachParallel(source,x=>Interlocked.Add(ref sum,x),null);Assert.Equal(source.Sum(x=>(long)x),sum);
        sum=0;await source.ForEachParallelAsync(x=>Interlocked.Add(ref sum,x));Assert.Equal(source.Sum(x=>(long)x),sum);
        var dictionary=ParallelMethods.ToDictionaryParallel(source,x=>new KeyValuePair<int,int>(x,x+1));
        foreach(int value in source)Assert.Equal(value+1,dictionary[value]);
        Assert.Equal(count,dictionary.Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParallelHonorsEitherCancellationSource(bool secondary)
    {
        using var first=new CancellationTokenSource();using var second=new CancellationTokenSource();
        if(secondary)second.Cancel();else first.Cancel();
        var parallel=new ParallelOptions{CancellationToken=second.Token,MaxDegreeOfParallelism=2};
        var options=new IIterableOptions{cancellationToken=first.Token,parallelOptions=parallel};
        Assert.ThrowsAny<OperationCanceledException>(()=>ParallelMethods.FilterParallel(Array.Empty<int>(),x=>true,options));
        Assert.ThrowsAny<OperationCanceledException>(()=>ParallelMethods.ForEachParallel((IEnumerable<int>)Array.Empty<int>(),x=>{},options));
        Assert.ThrowsAny<OperationCanceledException>(()=>ParallelMethods.ToDictionaryParallel(Array.Empty<int>(),x=>new KeyValuePair<int,int>(x,x),options));
        Assert.Same(parallel,options.parallelOptions);Assert.Equal(second.Token,parallel.CancellationToken);
    }

    [Fact]
    public async Task ParallelDoesNotMutateOptionsAndPropagatesCallbackErrors()
    {
        var options=new IIterableOptions();ParallelMethods.FilterParallel(new[]{1},x=>true,options);Assert.Null(options.parallelOptions);
        await Assert.ThrowsAsync<AggregateException>(async()=>await new[]{1}.ForEachParallelAsync(x=>throw new InvalidOperationException()));
        var duplicate=ParallelMethods.ToDictionaryParallel(new[]{1,1},x=>new KeyValuePair<int,int>(x,x));Assert.Single(duplicate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(32)]
    [InlineData(2048)]
    [InlineData(5000)]
    [InlineData(50000)]
    public void BothSortsHandleOrderedAndDuplicateKeys(int count)
    {
        var source=Enumerable.Range(0,count).ToArray();int calls=0;
        Assert.Equal(source,source.SortCollection(x=>{calls++;return x;}));Assert.InRange(calls,0,count);
        calls=0;Assert.Equal(source,ParallelMethods.SortCollectionParallel(source,x=>{Interlocked.Increment(ref calls);return x;}));Assert.InRange(calls,0,count);
        var input=source.Select(x=>(Id:x,Key:x%7)).ToArray();
        foreach(var direction in new[]{SortDirection.Ascending,SortDirection.Descending})
        {
            var sorted=ParallelMethods.SortCollectionParallel(input,x=>x.Key,direction).ToArray();
            var expected=direction==SortDirection.Ascending?input.OrderBy(x=>x.Key):input.OrderByDescending(x=>x.Key);
            Assert.Equal(expected.Select(x=>x.Key),sorted.Select(x=>x.Key));
            Assert.Equal(source,sorted.Select(x=>x.Id).Order());
        }
        Assert.Equal(source,input.Select(x=>x.Id));
    }

    [Fact]
    public void ParallelDictionaryKeepsDuplicateKeyContractForLargerInputs()
    {
        var result=ParallelMethods.ToDictionaryParallel(Enumerable.Range(0,1000).ToArray(),x=>new KeyValuePair<int,int>(x%7,x%7));
        Assert.Equal(7,result.Count);
        for(int key=0;key<7;key++)Assert.Equal(key,result[key]);
        Assert.IsType<System.Collections.Concurrent.ConcurrentDictionary<int,int>>(result);
    }

    [Fact]
    public void SortsStringKeysInBothDirections()
    {
        var input=new[]{ "zebra", "apple", "moon", "apple" };
        Assert.Equal(input.OrderBy(x=>x),input.SortCollection(x=>x));
        Assert.Equal(input.OrderByDescending(x=>x),ParallelMethods.SortCollectionParallel(input,x=>x,SortDirection.Descending));
    }

    [Fact]
    public void ParallelHonorsDegreeLimitAndCancellationDuringExecution()
    {
        int active=0,maximum=0;
        var options=new IIterableOptions{parallelOptions=new ParallelOptions{MaxDegreeOfParallelism=2}};
        ParallelMethods.ForEachParallel((IEnumerable<int>)Enumerable.Range(0,1000).ToArray(),x=>
        {
            int current=Interlocked.Increment(ref active);
            int observed;
            do { observed=Volatile.Read(ref maximum); if(observed>=current)break; }
            while(Interlocked.CompareExchange(ref maximum,current,observed)!=observed);
            Thread.SpinWait(100);
            Interlocked.Decrement(ref active);
        },options);
        Assert.InRange(maximum,1,2);
        using var cancel=new CancellationTokenSource();options.cancellationToken=cancel.Token;
        Assert.ThrowsAny<OperationCanceledException>(()=>ParallelMethods.FilterParallel(Enumerable.Range(0,1000).ToArray(),x=>{cancel.Cancel();return true;},options));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1000)]
    public void TypedArrayIndexedOperationsPreserveResults(int count)
    {
        var source=Enumerable.Range(0,count).ToArray();
        Assert.Equal(source.Sum(x=>(long)x)*2,source.Reduce((sum,x,i)=>sum+x+i,0L));
        var result=new List<int>();source.ForEach((x,i)=>result.Add(x+i));
        Assert.Equal(source.Select(x=>2*x),result);
    }
}
