# Performance: faster collection operations in 2.0

Version 2.0 brings substantial gains to everyday collection operations: filtering, mapping, iteration, slicing, dictionary construction, and sorting. Familiar APIs are backed by LINQ, specialized array paths, and more efficient algorithms.

**Highlights from the final comparison:** Map up to **9.89×**, Filter up to **4.61×**, Slice up to **12.43×**, and ForEach up to **3.94×** the baseline speed across tested inputs. The table below includes both comparison runs and all method families.

### Release coverage

Recorded on 2026-09-20 against the 1.0.2 source baseline (`fb87e6795ad60b353a2cd0012b9b42b892fd397f`). The measured implementation shipped in **2.0.0–2.0.2**: its library source hashes match the benchmark snapshot in [manifest.json](manifest.json). The snapshot was originally named `2.0.0-preview.1`; that is a historical benchmark label, not the current release status. Measurements compare source implementations in the same process.

## Measured speedups

**156 workloads, two final comparison runs.** Each range spans the tested inputs for that method. Ratios are baseline time divided by 2.0 time: 1.00× is parity and higher is faster. Ranges describe measured inputs rather than statistical confidence intervals.

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

The final comparison and repeat are the reference runs for this report.

## What makes 2.0 fast

- Non-indexed Filter and generalized Map directly return LINQ iterators. Indexed Filter has array/list paths without per-item optional-cancellation checks when cancellation is disabled.
- Non-indexed Reduce uses Aggregate; indexed array reductions and ForEach avoid interface enumerators/adapters. Indexed Map uses Select. Existing eager Concat array/list copy paths remain; general Concat uses LINQ when cancellation is absent.
- Slice uses Skip/Take partitions without cancellation, exploiting array/list copy paths and bounded streaming enumeration; cancellation uses a single bounded enumerator. No Count()+repeated ElementAt(). ToRecord pre-sizes known-size dictionaries without enumerating unknown streams and avoids non-indexed callback adapters.
- Sequential sorting caches keys and uses Array.Sort. Parallel sorting uses balanced range sorts and merges, with a sequential path below 2,048 items; both avoid the old quadratic pivot path. Keys are projected once for inputs of size >=2.
- Parallel dictionary callbacks run in parallel into local batches, then merge without inter-thread write contention into a ConcurrentDictionary. Known inputs <=64 retain direct insertion. Duplicate-key winner remains unspecified; duplicates do not throw.
- Parallel options are not mutated; either cancellation token is honored, with linking only when necessary. Default concurrency remains five. Async operations honor scheduling cancellation, finish their work before await returns, and surface callback errors through the task. New ForEachParallelAsync provides an awaitable alternative to the retained, obsolete void methods.

## Allocation measurements

Sorting caches keys to accelerate comparisons; parallel sorting and dictionary construction use additional buffers. At 5,000 integer items, sequential sorting allocates about 40 KB versus 20 KB in the baseline. Per-case synchronous allocations are included in the CSV; worker-thread allocations require separate instrumentation.

See the [API and migration guide](../docs/API.md) for execution contracts and upgrade details.

## Reproduce the measurements

.NET 8.0.31, Arm64, 10 reported logical processors, Release, SDK 8.0.425, tiered compilation disabled for every variant. Eight warmups per variant; three-operation pilot; adaptive batches targeting 5 ms (max 20,000 synchronous or 200 threaded operations); nine samples; medians reported. Variant order rotates per sample. Original source is preserved with renamed namespaces in Baseline/ so old and new execute in the same process. Baseline timing was captured before candidate edits.

Inputs: arrays, Lists and replayable iterators, 32/1,000/10,000 integers; sorts additionally use ordered, shuffled (seed 1729) and duplicate-heavy inputs at 32/1,000/5,000. Async and non-sort parallel timings use arrays; a deterministic 200-step arithmetic predicate supplements cheap parallel filtering. Sequence operations are consumed, with callback work/materialization included. Async measurements include task completion and result materialization for both implementations. Most wrapper benchmark call sites use IEnumerable; six additional cases explicitly exercise typed-array indexed Reduce and ForEach. Typed-array Map behavior has dedicated coverage in the behavior suite. The initial standalone baseline contains 150 cases; both final comparisons remeasure the original source for all 156 cases.

Results cover the machines, input shapes, sizes, and callbacks described above. The harness reports microbenchmark medians and includes native LINQ comparisons where semantically comparable. All current method families with measurable completion are covered; the awaitable replacement for legacy void async iteration is verified by behavior tests.

## Verification and reproduction

107 tests: original 50 tests plus 57 additional behavior cases. Coverage includes generic Map inference, mixed/empty sources, laziness, single-use slicing/disposal, ordering/membership, reductions, duplicate keys, async exceptions/cancellation, linked tokens/options, degree limits and sorting up to 50,000 elements. Three upstream parallel tests now use ConcurrentBag instead of concurrently writing List; two cancellation assertions were updated for the documented contract. The final redesigned implementation passed all 107 tests; see Results/final-redesign-tests.trx. The standalone library Release build also passed with zero warnings/errors.

From this repository with .NET 8 installed:

```sh
DOTNET_TieredCompilation=0 dotnet run --project Performance/Benchmarks.csproj -c Release -- baseline
DOTNET_TieredCompilation=0 dotnet run --project Performance/Benchmarks.csproj -c Release
dotnet test Performance/Tests/Tests.csproj -c Release
dotnet build collection-iterable-methods.csproj -c Release
```

Benchmark and test sources are excluded from the library package. The implementation is published on [NuGet](https://www.nuget.org/packages/collection-iterable-methods); this report preserves the original measurement environment and results.
