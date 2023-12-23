# collection-iterable-methods 🌟

![Collection Iterable Methods Logo](https://raw.githubusercontent.com/johnny-quesada-developer/global-hooks-example/main/public/avatar2.jpeg)

Welcome to **collection-iterable-methods** – your comprehensive solution for handling collections in .NET with both synchronous and asynchronous approaches! This library provides a wide range of methods for manipulating and querying collections, arrays, and enumerable types, giving you the power to write cleaner, more efficient, and responsive code.

**collection-iterable-methods** integrates seamlessly with your .NET projects, offering a fluent and intuitive API for common collection operations. It includes synchronous methods for immediate execution and asynchronous methods for non-blocking operations, ensuring improved scalability and performance in your applications.

## Features:

### Synchronous Methods (CollectionIterable):

- **Concat**: Combine two sequences efficiently.
- **Filter**: Filter elements based on conditions.
- **ForEach**: Perform actions on each element.
- **Map**: Transform elements from one form to another.
- **Reduce**: Aggregate elements to a single result.
- **Slice**: Take a subsequence of elements.
- **SortCollection**: Sort elements based on key selectors and directions.
- **ToRecord**: Convert collections to dictionaries based on key-value pair selectors.

### Asynchronous Methods (CollectionIterableAsync):

- **ConcatAsync**: Asynchronously combine two sequences.
- **FilterAsync**: Asynchronously filter elements based on conditions.
- **ForEachAsync**: Asynchronously perform actions on each element.

### Parallel Methods (CollectionIterableParallel - Use with caution):

- **FilterParallel**: Parallel filtering of elements.
- **ForEachParallel**: Parallel execution of actions on elements.
- **SortCollectionParallel**: Parallel sorting of collections.
- **ToDictionaryParallel**: Parallel conversion of collections to dictionaries.

## Getting Started:

To start using **collection-iterable-methods** in your project, include it as a dependency and begin enhancing your collection manipulation capabilities. Each method is designed to be intuitive and aligns with common functional programming patterns, making your .NET collection handling efficient and straightforward.

## Examples:

Here's a sneak peek of what you can do:

```csharp
// Synchronous usage with CollectionIterable
var numbers = new[] { 1, 2, 3, 4, 5 };
var evenNumbers = numbers.Filter(n => n % 2 == 0);
var doubledNumbers = numbers.Map(n => n * 2);
var sortedNumbers = numbers.SortCollection(n => n);

// Asynchronous usage with CollectionIterableAsync
var asyncDoubledNumbers = await numbers.MapAsync(n => n * 2);
var asyncFilteredNumbers = await numbers.FilterAsync(n => n > 2);
```

## Caution:

While the parallel methods in **CollectionIterableParallel** provide powerful tools for processing large data sets, they should be used with caution in synchronous contexts or smaller data sets due to the potential overhead and complexity. Always consider the nature of your data and task before choosing parallel methods.

## Contributions:

Contributions are welcome! If you'd like to improve **collection-iterable-methods**, feel free to fork the repository, make your changes, and submit a pull request. We appreciate any contributions that enhance the library's functionality and usability.

## License:

**collection-iterable-methods** is released under the MIT License. Feel free to use it in your personal and commercial projects.

Jumpstart your .NET collection handling with **collection-iterable-methods** and make your code more readable, maintainable, efficient, and responsive! 🌟
