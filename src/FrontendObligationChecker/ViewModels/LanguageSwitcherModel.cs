namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

[ExcludeFromCodeCoverage]
public class LanguageSwitcherModel
{
    public CultureInfo? CurrentCulture { get; set; }

    public List<CultureInfo>? SupportedCultures { get; set; }

    public string? ReturnUrl { get; set; }
}