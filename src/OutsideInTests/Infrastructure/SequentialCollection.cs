namespace OutsideInTests.Infrastructure;

/// <summary>
/// Shared stubs can't be used safely in parallel
/// so we use this collection definition to force all the
/// integration tests to run sequentially.
/// </summary>
[CollectionDefinition(Sequential, DisableParallelization = true)]
public class SequentialCollection
{
    public const string Sequential = nameof(Sequential);
}
