using System.Diagnostics;
using System.Text.Json;
using Old = PreviousCollectionIterable.PreviousCollectionIterable;
using Next = CollectionIterable.CollectionIterable;
using OldAsync = PreviousCollectionIterableAsync.PreviousCollectionIterableAsync;
using NextAsync = CollectionIterableAsync.CollectionIterableAsync;
using OldParallel = PreviousCollectionIterableParallel.PreviousCollectionIterableParallel;
using NextParallel = CollectionIterableParallel.CollectionIterableParallel;

bool baselineOnly = args.Contains("baseline");
var cases = new List<BenchCase>();
foreach (int size in new[] { 32, 1000, 10000 })
foreach (string shape in new[] { "array", "list", "iterator" })
{
    int[] input = Enumerable.Range(0, size).ToArray();
    IEnumerable<int> source = shape switch { "array" => input, "list" => input.ToList(), _ => Enumerate(input) };
    Add("Filter", () => Old.Filter(source, x => x % 3 == 0).ToArray().Length, () => Next.Filter(source, x => x % 3 == 0).ToArray().Length, () => source.Where(x => x % 3 == 0).ToArray().Length);
    Add("FilterIndexed", () => Old.Filter(source, (x,i) => i % 3 == 0).ToArray().Length, () => Next.Filter(source, (x,i) => i % 3 == 0).ToArray().Length, () => source.Where((x,i) => i % 3 == 0).ToArray().Length);
    Add("Map", () => Old.Map(source, x => x + 1).ToArray().Length, () => Next.Map(source, x => x + 1).ToArray().Length, () => source.Select(x => x + 1).ToArray().Length);
    Add("MapIndexed", () => Old.Map(source, (x,i) => x + i).ToArray().Length, () => Next.Map(source, (x,i) => x + i).ToArray().Length, () => source.Select((x,i) => x + i).ToArray().Length);
    Add("Reduce", () => Old.Reduce(source, (sum,x) => sum + x, 0L), () => Next.Reduce(source, (sum,x) => sum + x, 0L), () => source.Aggregate(0L, (sum,x) => sum + x));
    Add("ReduceIndexed", () => Old.Reduce(source, (sum,x,i) => sum + x + i, 0L), () => Next.Reduce(source, (sum,x,i) => sum + x + i, 0L));
    Add("ForEach", () => { long sum=0; Old.ForEach(source,x => sum+=x);return sum; }, () => { long sum=0; Next.ForEach(source,x => sum+=x);return sum; });
    Add("ForEachIndexed", () => { long sum=0; Old.ForEach(source,(x,i) => sum+=x+i);return sum; }, () => { long sum=0; Next.ForEach(source,(x,i) => sum+=x+i);return sum; });
    Add("Concat", () => Old.Concat(source,input).ToArray().Length, () => Next.Concat(source,input).ToArray().Length, () => Enumerable.Concat(source,input).ToArray().Length);
    int start = size / 2, end = start + 10;
    Add("Slice", () => Old.Slice(source,start,end).ToArray().Length, () => Next.Slice(source,start,end).ToArray().Length, () => source.Skip(start).Take(end-start).ToArray().Length);
    Add("ToRecord", () => Old.ToRecord(source,x => new KeyValuePair<int,int>(x,x+1)).Count, () => Next.ToRecord(source,x => new KeyValuePair<int,int>(x,x+1)).Count, () => source.ToDictionary(x=>x,x=>x+1).Count);
    Add("ToRecordIndexed", () => Old.ToRecord(source,(x,i) => new KeyValuePair<int,int>(x,i)).Count, () => Next.ToRecord(source,(x,i) => new KeyValuePair<int,int>(x,i)).Count);
    if (shape != "array") continue;
    Add("ReduceIndexedTypedArray", () => Old.Reduce(input, (sum,x,i) => sum+x+i, 0L), () => Next.Reduce(input, (sum,x,i) => sum+x+i, 0L));
    Add("ForEachIndexedTypedArray", () => {long sum=0;Old.ForEach(input,(x,i)=>sum+=x+i);return sum;}, () => {long sum=0;Next.ForEach(input,(x,i)=>sum+=x+i);return sum;});
    Add("FilterAsync", () => OldAsync.FilterAsync(source,x=>x%3==0).GetAwaiter().GetResult().ToArray().Length, () => NextAsync.FilterAsync(source,x=>x%3==0).GetAwaiter().GetResult().ToArray().Length, threaded:true);
    Add("ConcatAsync", () => OldAsync.ConcatAsync(source,source).GetAwaiter().GetResult().ToArray().Length, () => NextAsync.ConcatAsync(source,source).GetAwaiter().GetResult().ToArray().Length, threaded:true);
    Add("ForEachAsync", () => {long sum=0;OldAsync.ForEachAsync(source,x=>sum+=x).GetAwaiter().GetResult();return sum;}, () => {long sum=0;NextAsync.ForEachAsync(source,x=>sum+=x).GetAwaiter().GetResult();return sum;},threaded:true);
    Add("FilterParallel", () => OldParallel.FilterParallel(source,x=>x%3==0,null).Count(), () => NextParallel.FilterParallel(source,x=>x%3==0,null).Count(),threaded:true);
    Add("FilterParallelCpu", () => OldParallel.FilterParallel(source,CpuPredicate,null).Count(), () => NextParallel.FilterParallel(source,CpuPredicate,null).Count(),threaded:true);
    Add("FilterParallelAsync", () => OldAsync.FilterParallelAsync(source,x=>x%3==0).GetAwaiter().GetResult().Count(), () => NextAsync.FilterParallelAsync(source,x=>x%3==0).GetAwaiter().GetResult().Count(),threaded:true);
    Add("ForEachParallel", () => {long sum=0;OldParallel.ForEachParallel(source,x=>Interlocked.Add(ref sum,x),null);return sum;}, () => {long sum=0;NextParallel.ForEachParallel(source,x=>Interlocked.Add(ref sum,x),null);return sum;},threaded:true);
    Add("ToDictionaryParallel", () => OldParallel.ToDictionaryParallel(source,x=>new KeyValuePair<int,int>(x,x)).Count, () => NextParallel.ToDictionaryParallel(source,x=>new KeyValuePair<int,int>(x,x)).Count,threaded:true);
    void Add(string name, Func<long> previous, Func<long> candidate, Func<long>? native=null, bool threaded=false) => cases.Add(new(name,size,shape,previous,candidate,native,threaded));
}
foreach (int size in new[] { 32, 1000, 5000 })
foreach (string shape in new[] { "sorted", "shuffled", "duplicates" })
{
    var input = Enumerable.Range(0,size).ToArray();
    if(shape=="shuffled")new Random(1729).Shuffle(input);
    if(shape=="duplicates")for(int i=0;i<size;i++)input[i]%=8;
    cases.Add(new("SortCollection",size,shape,()=>Old.SortCollection(input,x=>x).Last(),()=>Next.SortCollection(input,x=>x).Last(),()=>input.OrderBy(x=>x).Last(),false));
    cases.Add(new("SortCollectionParallel",size,shape,()=>OldParallel.SortCollectionParallel(input,x=>x).Last(),()=>NextParallel.SortCollectionParallel(input,x=>x).Last(),null,true));
}
// Force full sorting in LINQ rather than its Last() selection optimization.
cases = cases.Select(c => c.Name=="SortCollection" ? c with { Native=null } : c).ToList();
var methodFilter = args.FirstOrDefault(argument => argument.StartsWith("--method="))?[9..];
if (methodFilter != null) cases = cases.Where(test => test.Name.StartsWith(methodFilter)).ToList();
var results = new List<object>();
foreach(var test in cases)
{
    var variants = new List<(string Name,Func<long> Operation)> { ("previous",test.Previous) };
    if(!baselineOnly){variants.Add(("candidate",test.Candidate));if(test.Native!=null)variants.Add(("native",test.Native));}
    var samples = variants.Select(_=>new List<double>()).ToArray();
    var allocations = variants.Select(_=>new List<double>()).ToArray();
    long expected=test.Previous();
    for(int v=0;v<variants.Count;v++)
    {
        if(variants[v].Operation()!=expected)throw new Exception($"Result mismatch: {test.Name}");
        for(int i=0;i<8;i++)variants[v].Operation();
    }
    var repeats=new int[variants.Count];
    for(int v=0;v<variants.Count;v++)
    {
        var pilot=Stopwatch.StartNew();for(int i=0;i<3;i++)variants[v].Operation();pilot.Stop();
        repeats[v]=Math.Clamp((int)(5000/Math.Max(0.01,pilot.Elapsed.TotalMicroseconds/3)),1,test.Threaded?200:20000);
    }
    long checksum=0;
    for(int sample=0;sample<9;sample++)
    for(int offset=0;offset<variants.Count;offset++)
    {
        int v=(sample+offset)%variants.Count; // Rotate order to reduce drift bias.
        long bytes=GC.GetAllocatedBytesForCurrentThread();
        long begin=Stopwatch.GetTimestamp();
        for(int i=0;i<repeats[v];i++)checksum^=variants[v].Operation();
        samples[v].Add(Stopwatch.GetElapsedTime(begin).TotalMicroseconds/repeats[v]);
        allocations[v].Add((GC.GetAllocatedBytesForCurrentThread()-bytes)/(double)repeats[v]);
    }
    GC.KeepAlive(checksum);
    for(int v=0;v<variants.Count;v++)
    {
        samples[v].Sort();allocations[v].Sort();
        results.Add(new{method=test.Name,size=test.Size,shape=test.Shape,variant=variants[v].Name,medianUs=samples[v][4],minUs=samples[v][0],maxUs=samples[v][^1],bytesPerOp=test.Threaded?(double?)null:allocations[v][4]});
    }
}
Console.WriteLine(JsonSerializer.Serialize(new{runtime=System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,architecture=System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString(),processors=Environment.ProcessorCount,results},new JsonSerializerOptions{WriteIndented=true}));
static IEnumerable<int> Enumerate(int[] input){foreach(int value in input)yield return value;}
static bool CpuPredicate(int value){uint state=(uint)value;for(int i=0;i<200;i++)state=unchecked(state*1664525+1013904223);return state%3==0;}
record BenchCase(string Name,int Size,string Shape,Func<long> Previous,Func<long> Candidate,Func<long>? Native,bool Threaded);
