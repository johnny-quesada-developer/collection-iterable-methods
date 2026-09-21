# Development guide

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
