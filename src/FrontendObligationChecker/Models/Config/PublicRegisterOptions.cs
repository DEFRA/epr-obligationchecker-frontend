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

    public string PublicRegisterNextYearStartMonthAndDay { get; set; }

    public string? PublicRegisterPreviousYearEndMonthAndDay { get; set; }

    public string? CurrentYear { get; set; }
}