using System.Globalization;

namespace FrontendObligationChecker.ViewModels;
public class CookieBannerModel
{
    public bool ShowBanner { get; set; }

    public bool ShowAcknowledgement { get; set; }

    public bool AcceptAnalytics { get; set; }

    public string? ReturnUrl { get; set; }
}