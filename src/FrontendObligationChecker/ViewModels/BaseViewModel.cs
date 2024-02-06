namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class BaseViewModel
{
    public string BackLinkToDisplay { get; set; } = string.Empty;

    public string CurrentPage { get; set; }
}