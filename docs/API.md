# API behavior and upgrading

[Website](https://johnny-quesada-developer.github.io/collection-iterable-methods/) · [Getting started](../README.md)

## Contracts

| Methods | Execution / ordering |
|---|---|
| Filter, Map | Lazy, ordered, replayable when the source is replayable |
| Slice | Lazy, ordered, start inclusive/end exclusive; negative start rejected |
| Reduce, ForEach, ToRecord, SortCollection | Complete synchronously |
| Concat | Arrays/collections copied immediately; general enumerables lazy |
| FilterAsync, ConcatAsync, ForEachAsync | Synchronous callbacks/work run on Task.Run; complete before await returns |
| FilterParallel, FilterParallelAsync | Complete before return/await; **unordered** output |
| ForEachParallel, ForEachParallelAsync | Concurrent callbacks; caller must synchronize shared mutable state |
| SortCollectionParallel | Sorted output; equal-key order unspecified; small inputs may sort sequentially |
| ToDictionaryParallel | Concurrent callbacks; duplicate-key winner unspecified; returns a ConcurrentDictionary through IDictionary |

Both indexed and non-indexed callbacks are supported for ordinary transformations. Map supports different input/output types. ToRecord rejects duplicate keys. Sorts preserve source contents and are not stable for equal keys. Selectors should be pure.

Async callbacks remain synchronous delegates: these methods are CPU offloading helpers, not async-I/O pipelines. Do not pass async-void lambdas. Use native async APIs for I/O and async streams. Cancellation is cooperative; running callbacks cannot be forcibly interrupted. Parallel options are copied, not mutated; either supplied token can cancel the operation. Default maximum parallelism is five.

The legacy void `CollectionIterableAsync.ForeachParallel` overloads remain but are obsolete. Use `ForEachParallelAsync` and await it to observe completion and errors.

## Migration from 1.0.2

- Rebuild consumers: Map now uses `<TSource,TResult>`. Inferred calls such as `.Map(x => x + 1)` remain valid. Explicit `.Map<int>(...)` becomes `.Map<int,int>(...)` or omits type arguments.
- FilterAsync and general-enumerable ConcatAsync now materialize before completing; work/errors no longer wait until result enumeration. This changes timing and memory lifetime.
- Registered parallel cancellation raises OperationCanceledException directly instead of the previous manual-check AggregateException. Callback failures remain observable; dictionary merge/key failures may now surface directly after callback processing.
- Framework-backed methods use framework argument validation; sorts cache keys and can change comparator exception wrapping, null-key handling and tie order.


## Implementation and allocation details

- **Map is a deliberate major-version API change:** six overloads now use `<TSource,TResult>`. Ordinary inferred calls keep working; `Map<int>(...)` becomes `Map<int,int>(...)` or drops explicit type arguments. Rebuild binary consumers; do not replace a 1.0.2 DLL in place.
- Filter/Map remain lazy and ordered; parallel results remain unordered. LINQ-backed methods validate null arguments using framework behavior. Slice validates arguments eagerly and rejects negative start consistently instead of inconsistent old errors. Sorting is still unstable for equal keys; selector call count/order and comparator exception wrapping can change.
- FilterAsync and enumerable ConcatAsync now materialize before completing. This fixes misleading completion but changes evaluation timing and memory lifetime. Callbacks are synchronous CPU callbacks, not asynchronous I/O callbacks; do not pass async-void lambdas. Parallel pre-cancellation now throws OperationCanceledException directly rather than the earlier manual-check AggregateException.
- Key caching costs O(n) extra memory; at 5,000 integer items sequential sorting uses about 40 KB instead of 20 KB. Parallel sorting and dictionary batching also use additional buffers. Synchronous per-case allocation measurements are in CSV. Threaded allocation totals are intentionally omitted: current-thread counters miss worker allocations.
- No runtime dependency was added. This remains an in-memory IEnumerable library. Use Queryable directly for database expressions and native asynchronous APIs for I/O/async streams. Public method return types other than generalized Map remain unchanged. ConcurrentDictionary results remain ConcurrentDictionary instances.

