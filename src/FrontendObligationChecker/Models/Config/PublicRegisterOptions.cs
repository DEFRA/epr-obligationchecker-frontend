namespace FrontendObligationChecker.Models.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class PublicRegisterOptions
{
    public const string ConfigSection = "PublicRegister";

    public string? PublicRegisterBlobContainerName { get; set; }

    public string? PublicRegisterCsoBlobContainerName { get; set; }

    public string? EnforcementActionsBlobContainerName { get; set; }

    public string? EnforcementActionFileName { get; set; }

    public DateTime PublishedDate { get; set; }

    /// <summary>
    /// Registers for this year and earlier are shown on the archive page instead of the
    /// main public register (e.g. 2025). When null, nothing is archived.
    /// </summary>
    public int? ArchivedUpToYear { get; set; }

    public string? CurrentYear { get; set; }

    /// <summary>
    /// Override DateTime.UtcNow for testing/QA. Format: yyyy-MM-dd (e.g. "2025-12-08").
    /// When null, the real clock is used.
    /// </summary>
    public string? FakeDateTimeUtcNow { get; set; }
}