﻿namespace FrontendObligationChecker.Models.Config;

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

    public string StartPage { get; set; }

    public string GovUkHome { get; set; }
}