namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class PhaseBannerModel
{
    public string Status { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public bool ShowBanner { get; set; }
}