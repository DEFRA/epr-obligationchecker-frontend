namespace FrontendObligationChecker.Models.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ExternalUrlsOptions
{
    public const string ConfigSection = "ExternalUrls";

    public string EprGuidance { get; set; }

    public string PrivacyDataProtectionPublicRegister { get; set; }

    public string PrivacyWebBrowser { get; set; }

    public string PrivacyGoogleAnalytics { get; set; }

    public string PrivacyEuropeanEconomicArea { get; set; }

    public string PrivacyDefrasPersonalInformationCharter { get; set; }

    public string PrivacyInformationCommissioner { get; set; }

    public string PrivacyFindOutAboutCallCharges { get; set; }

    public string PrivacyScottishEnvironmentalProtectionAgency { get; set; }

    public string PrivacyNationalResourcesWales { get; set; }

    public string PrivacyNorthernIrelandEnvironmentAgency { get; set; }

    public string PrivacyEnvironmentAgency { get; set; }

    public string StartPage { get; set; }

    public string GovUkHome { get; set; }

    public string AccessibilityAbilityNet { get; set; }

    public string AccessibilityEqualityAdvisorySupportService { get; set; }

    public string AccessibilityContactUs { get; set; }

    public string AccessibilityWebContentAccessibility { get; set; }

    public string WhoIsAffectedAndWhatToDo { get; set; }

    public string NpwdRegister { get; set; }

    public string PrivacyPage { get; set; }

    public string AccessibilityPage { get; set; }
}