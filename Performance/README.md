# Performance report: 1.0.2 source → 2.0.0-preview.1 candidate

2026-09-20. Local, unpublished candidate. Original baseline is source commit `fb87e6795ad60b353a2cd0012b9b42b892fd397f`, including the original sorting algorithm. The earlier sequential-sort improvement is included in this comparison. These are source-to-source measurements, not measurements of the published NuGet binary.

**Result:** clear gains in ordinary filtering, mapping, iteration, streaming slicing, dictionary creation and sorting. Other paths are roughly unchanged or fluctuate; no claim that every method is faster. **107 behavior tests pass.**

## Measured speedups

156 workloads, two final comparison runs. Each range spans the inputs tested for that method. **1.00× = unchanged; below 1.00× = slower.** These ranges are not confidence intervals.

| Method | Final run | Repeat |
|---|---:|---:|
| Filter | 1.35–4.61× | 1.38–4.63× |
| FilterIndexed | 1.05–1.90× | 1.05–1.86× |
| Map | 2.31–9.89× | 2.14–10.29× |
| MapIndexed | 0.99–1.07× | 0.98–1.08× |
| Reduce | 1.08–1.39× | 1.12–1.36× |
| ReduceIndexed | 0.99–1.01× | 0.98–1.04× |
| ForEach | 1.49–3.94× | 1.64–3.78× |
| ForEachIndexed | 0.95–1.02× | 0.97–1.02× |
| Concat | 0.95–2.74× | 0.95–2.70× |
| Slice | 1.59–12.43× | 1.68–12.40× |
| ToRecord | 1.02–1.79× | 0.92–1.80× |
| ToRecordIndexed | 0.96–1.79× | 1.00–1.76× |
| ReduceIndexedTypedArray | 2.67–2.97× | 2.66–2.82× |
| ForEachIndexedTypedArray | 1.45–1.51× | 1.48–1.59× |
| FilterAsync | 1.06–2.37× | 1.09–2.70× |
| ConcatAsync | 0.96–0.99× | 0.92–0.99× |
| ForEachAsync | 1.42–1.48× | 1.08–1.69× |
| FilterParallel | 0.91–1.01× | 0.93–1.05× |
| FilterParallelCpu | 0.95–1.00× | 0.94–0.99× |
| FilterParallelAsync | 0.92–0.98× | 0.97–1.41× |
| ForEachParallel | 0.92–1.03× | 0.96–0.98× |
| ToDictionaryParallel | 1.13–2.79× | 1.03–2.36× |
| SortCollection | 1.61–658.56× | 1.67–644.36× |
| SortCollectionParallel | 1.96–569.88× | 2.02–581.55× |

[All individual timings](Results/comparison.md) · [CSV, allocation and LINQ comparisons](Results/comparison.csv) · [Initial baseline](Results/before.json) · [First comparison](Results/retained-final.json) · [Repeat](Results/retained-repeat.json).

The redesign retains typed-array indexed fast paths while restoring the original generic indexed loops. Slice now uses LINQ partitions without cancellation and one bounded enumerator with cancellation. The parallel-filter scratch-buffer experiment improved cheap predicates but slowed CPU-heavy callbacks by about 8–9%; it was rejected. Default parallel filtering retains the original ConcurrentBag algorithm.

**Not every row is an optimization.** Generic indexed loops are essentially unchanged. MapIndexed is retained for the approved type generalization. Async completion/cancellation and configured parallel options include correctness fixes. ConcatAsync measures 0.92–0.99× across these runs; no performance gain is claimed. Some parallel paths also measure slower. These results remain visible rather than being counted as wins; small differences cannot automatically be dismissed as noise or attributed to a particular change. The streaming ToRecord case at 32 elements is 1.02× in the final run and 0.98× in the repeat, so no repeatable gain is established there.

Historical after.json/confirmation.json and intermediate experiment files are preserved but superseded by retained-final.json and retained-repeat.json.

## Changes retained

- Non-indexed Filter and generalized Map directly return LINQ iterators. Indexed Filter has array/list paths without per-item optional-cancellation checks when cancellation is disabled.
- Non-indexed Reduce uses Aggregate; indexed array reductions and ForEach avoid interface enumerators/adapters. Indexed Map uses Select. Existing eager Concat array/list copy paths remain; general Concat uses LINQ when cancellation is absent.
- Slice uses Skip/Take partitions without cancellation, exploiting array/list copy paths and bounded streaming enumeration; cancellation uses a single bounded enumerator. No Count()+repeated ElementAt(). ToRecord pre-sizes known-size dictionaries without enumerating unknown streams and avoids non-indexed callback adapters.
- Sequential sorting caches keys and uses Array.Sort. Parallel sorting uses balanced range sorts and merges, with a sequential path below 2,048 items; both avoid the old quadratic pivot path. Keys are projected once for inputs of size >=2.
- Parallel dictionary callbacks run in parallel into local batches, then merge without inter-thread write contention into a ConcurrentDictionary. Known inputs <=64 retain direct insertion. Duplicate-key winner remains unspecified; duplicates do not throw.
- Parallel options are not mutated; either cancellation token is honored, with linking only when necessary. Default concurrency remains five. Async operations honor scheduling cancellation, finish their work before await returns, and surface callback errors through the task. New ForEachParallelAsync provides an awaitable alternative to the retained, obsolete void methods.

## Costs and compatibility

- **Map is a deliberate major-version API change:** six overloads now use `<TSource,TResult>`. Ordinary inferred calls keep working; `Map<int>(...)` becomes `Map<int,int>(...)` or drops explicit type arguments. Rebuild binary consumers; do not replace a 1.0.2 DLL in place.
- Filter/Map remain lazy and ordered; parallel results remain unordered. LINQ-backed methods validate null arguments using framework behavior. Slice validates arguments eagerly and rejects negative start consistently instead of inconsistent old errors. Sorting is still unstable for equal keys; selector call count/order and comparator exception wrapping can change.
- FilterAsync and enumerable ConcatAsync now materialize before completing. This fixes misleading completion but changes evaluation timing and memory lifetime. Callbacks are synchronous CPU callbacks, not asynchronous I/O callbacks; do not pass async-void lambdas. Parallel pre-cancellation now throws OperationCanceledException directly rather than the earlier manual-check AggregateException.
- Key caching costs O(n) extra memory; at 5,000 integer items sequential sorting uses about 40 KB instead of 20 KB. Parallel sorting and dictionary batching also use additional buffers. Synchronous per-case allocation measurements are in CSV. Threaded allocation totals are intentionally omitted: current-thread counters miss worker allocations.
- No runtime dependency was added. This remains an in-memory IEnumerable library. Use Queryable directly for database expressions and native asynchronous APIs for I/O/async streams. Public method return types other than generalized Map remain unchanged. ConcurrentDictionary results remain ConcurrentDictionary instances.

## Methodology and limits

.NET 8.0.31, Arm64, 10 reported logical processors, Release, SDK 8.0.425, tiered compilation disabled for every variant. Eight warmups per variant; three-operation pilot; adaptive batches targeting 5 ms (max 20,000 synchronous or 200 threaded operations); nine samples; medians reported. Variant order rotates per sample. Original source is preserved with renamed namespaces in Baseline/ so old and new execute in the same process. Baseline timing was captured before candidate edits.

Inputs: arrays, Lists and replayable iterators, 32/1,000/10,000 integers; sorts additionally use ordered, shuffled (seed 1729) and duplicate-heavy inputs at 32/1,000/5,000. Async and non-sort parallel timings use arrays; a deterministic 200-step arithmetic predicate supplements cheap parallel filtering. Sequence operations are consumed, with callback work/materialization included. Async measurements block on task completion and consume results, so the old lazy FilterAsync cannot look faster merely by returning an unevaluated iterator. Most wrapper benchmark call sites use IEnumerable; six additional cases explicitly exercise typed-array indexed Reduce and ForEach. Typed array Map behavior is tested separately because the original array constraint was defective. The initial standalone baseline contains 150 cases; both final comparisons remeasure the original source for all 156 cases.

These are reproducible microbenchmarks, not BenchmarkDotNet statistical estimates or end-to-end game measurements. They do not establish performance for other machines, key types, callback costs, database queries, Unity, or every overload. Native LINQ columns exist where semantically comparable; there is no claim that custom code universally beats LINQ. The obsolete void async ForeachParallel cannot provide a reliable task-completion timing and is excluded; its awaitable replacement is behavior-tested. All current method families with measurable completion are covered.

## Verification and reproduction

107 tests: original 50 tests plus 57 additional behavior cases. Coverage includes generic Map inference, mixed/empty sources, laziness, single-use slicing/disposal, ordering/membership, reductions, duplicate keys, async exceptions/cancellation, linked tokens/options, degree limits and sorting up to 50,000 elements. Three upstream parallel tests now use ConcurrentBag instead of concurrently writing List; two cancellation assertions were updated for the documented contract. The final redesigned implementation passed all 107 tests; see Results/final-redesign-tests.trx. The standalone library Release build also passed with zero warnings/errors.

From this repository with .NET 8 installed:

```sh
DOTNET_TieredCompilation=0 dotnet run --project Performance/Benchmarks.csproj -c Release -- baseline
DOTNET_TieredCompilation=0 dotnet run --project Performance/Benchmarks.csproj -c Release
dotnet test Performance/Tests/Tests.csproj -c Release
dotnet build collection-iterable-methods.csproj -c Release
```

The session used `/tmp/elora-dotnet/dotnet`, DOTNET_CLI_HOME=/tmp/elora-dotnet-home and NUGET_PACKAGES=/tmp/elora-nuget, plus -p:UseSharedCompilation=false. Benchmark/test sources are excluded from the library build. No package was published and Elora's dependency was not changed.
