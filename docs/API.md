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

Async collection methods run synchronous CPU callbacks on a worker thread. Use synchronous delegates with these methods and native async APIs for I/O. Cancellation is cooperative and observed between units of work. Parallel options are copied; either supplied token can cancel the operation. Default maximum parallelism is five.

Use `ForEachParallelAsync` and await it to observe completion and errors. The legacy void `CollectionIterableAsync.ForeachParallel` overloads are retained for compatibility and marked obsolete.

## Migration from 1.0.2

- Rebuild consumers: Map now uses `<TSource,TResult>`. Inferred calls such as `.Map(x => x + 1)` remain valid. Explicit `.Map<int>(...)` becomes `.Map<int,int>(...)` or omits type arguments.
- FilterAsync and general-enumerable ConcatAsync now materialize before completing; work/errors no longer wait until result enumeration. This changes timing and memory lifetime.
- Registered parallel cancellation raises OperationCanceledException directly instead of the previous manual-check AggregateException. Callback failures remain observable; dictionary merge/key failures may now surface directly after callback processing.
- Framework-backed methods use framework argument validation; sorts cache keys and can change comparator exception wrapping, null-key handling and tie order.
