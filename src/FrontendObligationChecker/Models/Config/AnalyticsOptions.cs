namespace FrontendObligationChecker.Models.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AnalyticsOptions
{
    public const string ConfigSection = "GoogleAnalytics";

    public string CookiePrefix { get; set; }

    public string MeasurementId { get; set; }

    public string TagManagerContainerId { get; set; }

    public string DefaultCookieName
    {
        get
        {
            return CookiePrefix;
        }
    }

    public string AdditionalCookieName
    {
        get
        {
            return $"{CookiePrefix}_{MeasurementId}";
        }
    }
}