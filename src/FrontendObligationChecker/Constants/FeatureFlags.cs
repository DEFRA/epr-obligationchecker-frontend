namespace FrontendObligationChecker.Constants;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class FeatureFlags
{
    public const string ObligationCheckerEnabled = "ObligationCheckerEnabled";
    public const string EnglandNationDownloadLink = "EnglandNationDownloadLink";
    public const string NorthernIrelandNationDownloadLink = "NorthernIrelandNationDownloadLink";
    public const string ScotlandNationDownloadLink = "ScotlandNationDownloadLink";
    public const string WalesNationDownloadLink = "WalesNationDownloadLink";
    public const string AllNationsDownloadLink = "AllNationsDownloadLink";
    public const string LargeProducerRegisterEnabled = "LargeProducerRegisterEnabled";
    public const string PublicRegisterEnabled = "PublicRegisterEnabled";
    public const string ComplianceSchemesRegisterEnabled = "ComplianceSchemesRegisterEnabled";
    public const string EnforcementActionsSectionEnabled = "EnforcementActionsSectionEnabled";
    public const string PublicRegisterNextYearEnabled = "PublicRegisterNextYearEnabled";
}