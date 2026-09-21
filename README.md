# collection-iterable-methods

**Write expressive C#. Get more from every collection.**

[![NuGet](https://img.shields.io/nuget/v/collection-iterable-methods.svg?color=24634b)](https://www.nuget.org/packages/collection-iterable-methods)
[![Target framework](https://img.shields.io/badge/.NET-8.0-1d5578)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-24634b)](LICENSE)

Bring the simplicity of `Filter`, `Map`, and `Reduce` to .NET 8. Turn arrays, lists, and enumerables into readable pipelines with automatic type inference, LINQ integration, and powerful async and parallel operations. **Zero package dependencies.**

**[Explore the website](https://johnny-quesada-developer.github.io/collection-iterable-methods/)** · [Install from NuGet](https://www.nuget.org/packages/collection-iterable-methods) · [API guide](https://github.com/johnny-quesada-developer/collection-iterable-methods/blob/main/docs/API.md) · [About the developer](https://johnny-quesada-developer.github.io/react-global-state-hooks/about/)

## Why use it?

- **Express the transformation.** Filter, map, slice, sort, and reduce with familiar names and concise, chainable code.
- **Keep the power of C#.** Transform between types with full inference, use indexed callbacks, and compose with LINQ.
- **Put concurrency to work.** Await CPU operations, run callbacks in parallel, and configure cancellation and concurrency in one place.
- **Benefit from optimized paths.** LINQ-backed transformations, bounded streaming slices, and cached sort keys improve core collection operations.

## Faster where it counts

Selected operations show substantial improvements in the recorded 2.0 candidate vs. 1.0.2 source benchmarks:

| Operation | Measured speedup | Repeat run |
|---|---:|---:|
| Map | **2.31–9.89×** | 2.14–10.29× |
| Filter | **1.35–4.61×** | 1.38–4.63× |
| Slice | **1.59–12.43×** | 1.68–12.40× |
| ForEach | **1.49–3.94×** | 1.64–3.78× |

Ranges cover the tested inputs for each operation. Explore all **156 workloads**, methodology, and allocation measurements in the [full benchmark report](https://github.com/johnny-quesada-developer/collection-iterable-methods/blob/main/Performance/README.md).

## Install

Requires a .NET 8-compatible project. Current stable release: **2.0.2**.

```sh
dotnet add package collection-iterable-methods --version 2.0.2
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
| Expressive collection pipelines | `CollectionIterable` | `Filter`, `Map`, `Reduce`, `Slice` |
| Awaitable CPU operations | `CollectionIterableAsync` | `FilterAsync`, `ForEachAsync` |
| Concurrent CPU callbacks | `CollectionIterableParallel` | `FilterParallel`, `ForEachParallel` |
| Cancellation and concurrency options | `CollectionIterableUtils` | `IIterableOptions` |

### Put async to work

```csharp
using CollectionIterableAsync;

var evens = await new[] { 1, 2, 3, 4 }.FilterAsync(n => n % 2 == 0);
// Work is complete; evens contains 2, 4.
```

### Take control of parallel execution

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

## Everything you need to keep building

Explore the [API and migration guide](https://github.com/johnny-quesada-developer/collection-iterable-methods/blob/main/docs/API.md) for execution behavior, ordering, cancellation, and upgrading from 1.0.2. Async methods accept synchronous CPU callbacks; use native async APIs for I/O. Parallel filter results are unordered.

Want to contribute? See the [development guide](https://github.com/johnny-quesada-developer/collection-iterable-methods/blob/main/docs/CONTRIBUTING.md) for builds, tests, benchmarks, and website deployment.

Created by [Johnny Quesada](https://johnny-quesada-developer.github.io/react-global-state-hooks/about/). Open source under the [MIT license](LICENSE).
