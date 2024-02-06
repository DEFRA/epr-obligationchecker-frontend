namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CookieDetailViewModel : BaseViewModel
{
    public bool CookiesAccepted { get; set; }

    public bool ShowAcknowledgement { get; set; }

    public string CookiePolicyCookieName { get; set; }

    public string SessionCookieName { get; set; }

    public string AntiForgeryCookieName { get; set; }

    public string GoogleAnalyticsDefaultCookieName { get; set; }

    public string GoogleAnalyticsAdditionalCookieName { get; set; }

    public string? ReturnUrl { get; set; }
}