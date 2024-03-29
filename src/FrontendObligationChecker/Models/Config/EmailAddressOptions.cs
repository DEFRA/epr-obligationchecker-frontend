﻿namespace FrontendObligationChecker.Models.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class EmailAddressOptions
{
    public const string ConfigSection = "EmailAddresses";

    public string DataProtection { get; set; }

    public string DefraGroupProtectionOfficer { get; set; }

    public string InformationCommissioner { get; set; }

    public string DefraHelpline { get; set; }
}