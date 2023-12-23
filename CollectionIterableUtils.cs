namespace CollectionIterableUtils
{
    public class IIterableOptions
    {
        public ParallelOptions? parallelOptions { get; set; }

        public CancellationToken? cancellationToken { get; set; }
    }
}