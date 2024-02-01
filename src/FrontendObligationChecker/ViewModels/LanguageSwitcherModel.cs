using System.Globalization;

namespace FrontendObligationChecker.ViewModels;

public class LanguageSwitcherModel
{
    public CultureInfo? CurrentCulture { get; set; }

    public List<CultureInfo>? SupportedCultures { get; set; }

    public string? ReturnUrl { get; set; }
}