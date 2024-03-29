﻿namespace FrontendObligationChecker.Models.Config;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class PhaseBannerOptions
{
    public const string ConfigSection = "BANNER_OPTIONS";

    public string ApplicationStatus { get; set; } = string.Empty;

    public string SurveyUrl { get; set; } = string.Empty;

    public bool Enabled { get; set; }
}