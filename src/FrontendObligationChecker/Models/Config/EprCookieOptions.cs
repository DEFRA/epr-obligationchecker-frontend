namespace FrontendObligationChecker.Models.Config;

public class EprCookieOptions
{
    public const string ConfigSection = "COOKIE_OPTIONS";

    public string SessionCookieName { get; set; }

    public string CookiePolicyCookieName { get; set; }

    public string AntiForgeryCookieName { get; set; }
}