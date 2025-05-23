﻿namespace FrontendObligationChecker.Constants;

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
    internal const string PublicRegisterEnabled = "PublicRegisterEnabled";

}