# collection-iterable-methods

**Familiar collection methods. Native C#.**

[![NuGet](https://img.shields.io/nuget/v/collection-iterable-methods.svg?color=24634b)](https://www.nuget.org/packages/collection-iterable-methods)
[![Target framework](https://img.shields.io/badge/.NET-8.0-1d5578)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-24634b)](LICENSE)

Node-style `Filter`, `Map`, `Reduce`, and more for .NET 8 collections. Use strongly typed transformations on arrays, collections, and enumerables, with explicit sequential, async, and parallel execution contracts. No package dependencies.

**[Website & guide](https://johnny-quesada-developer.github.io/collection-iterable-methods/)** · [NuGet](https://www.nuget.org/packages/collection-iterable-methods) · [API overview](https://johnny-quesada-developer.github.io/collection-iterable-methods/#reference) · [About the developer](https://johnny-quesada-developer.github.io/react-global-state-hooks/about/)

## Install

Requires a .NET 8-compatible project. Current stable release: **2.0.1**.

```sh
dotnet add package collection-iterable-methods --version 2.0.1
```

## A first transformation

```csharp
using CollectionIterable;

var numbers = new[] { 1, 2, 3, 4, 5, 6 };
var labels = numbers
    .Filter(number => number % 2 == 0)
    .Map(number => $"Item {number}")
    .ToArray();

// ["Item 2", "Item 4", "Item 6"]

var total = numbers.Reduce((sum, number) => sum + number, 0); // 21
var middle = numbers.Slice(1, 4).ToArray();                   // [2, 3, 4]
var indexed = numbers.Map((number, index) => $"{index}: {number}");
```

`Filter`, `Map`, and `Slice` are lazy. Enumerate to execute them; call `ToArray()` to capture a result. `Map` infers both the input and output type. Ordinary LINQ methods can follow these operations.

## Choose an execution model

| Use case | Namespace | Start with |
|---|---|---|
| Ordinary in-memory transformations | `CollectionIterable` | `Filter`, `Map`, `Reduce`, `Slice` |
| Offloading synchronous CPU work | `CollectionIterableAsync` | `FilterAsync`, `ForEachAsync` |
| Concurrent CPU callbacks | `CollectionIterableParallel` | `FilterParallel`, `ForEachParallel` |
| Cancellation and concurrency options | `CollectionIterableUtils` | `IIterableOptions` |

### Await completed work

```csharp
using CollectionIterableAsync;

var evens = await new[] { 1, 2, 3, 4 }.FilterAsync(n => n % 2 == 0);
// Work is complete; evens contains 2, 4.
```

### Bound parallelism and support cancellation

```csharp
using CollectionIterableParallel;
using CollectionIterableUtils;

using var cancellation = new CancellationTokenSource();
var options = new IIterableOptions
{
    parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 },
    cancellationToken = cancellation.Token
};
var evens = new[] { 1, 2, 3, 4 }.FilterParallel(n => n % 2 == 0, options);
// Contains 2, 4. Order is unspecified.
```

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

## Performance and scope

The [benchmark report](Performance/README.md) compares 156 workloads across two final runs, with timings, allocations, and reproduction commands. These are historical source-to-source measurements of the 2.0 preview candidate against 1.0.2 source, not measurements of published NuGet binaries.

Gains depend on method and input. Some async and parallel paths are unchanged or slower; extra buffers trade memory for speed. Use the report to choose representative workloads, then measure your application.

This library operates on in-memory `IEnumerable` collections. Use `Queryable` directly for database expression trees to retain provider translation, and native async APIs or async streams for I/O.

## Build and verify

With the .NET 8 SDK installed:

```sh
dotnet build collection-iterable-methods.csproj -c Release
dotnet test collection-iterable-methods.csproj -c test
dotnet test Performance/Tests/Tests.csproj -c Release
```

The extended test project also includes the original tests. The release validation passed 50 tests in the original suite and 107 in the extended suite.

Reproduce the benchmarks:

```sh
DOTNET_TieredCompilation=0 dotnet run --project Performance/Benchmarks.csproj -c Release
```

## Website

The site lives in `website/`: static HTML, CSS, and a small script for accessible example tabs and copy buttons. No Node dependencies or build step are needed. Its palette follows the sibling react-global-state-hooks website; the developer link goes to that site's About page.

Preview locally:

```sh
python3 -m http.server 8080 --directory website
```

The GitHub Pages workflow publishes only `website/` when its files change on `main`. Repository settings must use **GitHub Actions** as the Pages source. The workflow can also be run manually.

## Contributing and license

Issues and focused pull requests are welcome. Include a reproduction and the expected execution behavior for bug reports; include measurements and allocation tradeoffs for performance changes.

Created by [Johnny Quesada](https://johnny-quesada-developer.github.io/react-global-state-hooks/about/). Licensed under [MIT](LICENSE).
