namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;
using FrontendObligationChecker.Extensions;

[ExcludeFromCodeCoverage]
public class BaseViewModel
{
    public string BackLinkToDisplay { get; set; } = string.Empty;

    public string CurrentPage { get; set; }

    public string Timestamp { get; set; } = DateTime.UtcNow.UtcToGmt().ToString();
}