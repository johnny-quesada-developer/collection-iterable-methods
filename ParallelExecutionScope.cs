namespace CollectionIterableUtils;

// Snapshot caller options without mutating them; honor either cancellation source.
internal sealed class ParallelExecutionScope : IDisposable
{
    private readonly CancellationTokenSource? linkedCancellation;
    internal ParallelOptions Options { get; }

    internal ParallelExecutionScope(IIterableOptions? options)
    {
        var first = options?.cancellationToken ?? default;
        var second = options?.parallelOptions?.CancellationToken ?? default;
        CancellationToken cancellation = first.CanBeCanceled ? first : second;
        if (first.CanBeCanceled && second.CanBeCanceled && first != second)
        {
            linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(first, second);
            cancellation = linkedCancellation.Token;
        }
        Options = new ParallelOptions
        {
            MaxDegreeOfParallelism = options?.parallelOptions?.MaxDegreeOfParallelism ?? 5,
            TaskScheduler = options?.parallelOptions is { } parallel ? parallel.TaskScheduler : TaskScheduler.Default,
            CancellationToken = cancellation
        };
    }

    public void Dispose() => linkedCancellation?.Dispose();
}
