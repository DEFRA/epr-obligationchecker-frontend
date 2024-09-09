namespace FrontendObligationChecker.Models.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CachingOptions
{
    public const string ConfigSection = "Caching";

    public int ProducerReportFileSizeDays { get; init; }

    public int LargeProducersFileGeneratedHour { get; init; }

    public int LargeProducersFileGenerationWaitMinutes { get; init; }
}