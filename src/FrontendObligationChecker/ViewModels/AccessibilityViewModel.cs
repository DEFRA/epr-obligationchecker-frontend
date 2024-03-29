﻿namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AccessibilityViewModel : BaseViewModel
{
    public string AbilityNetUrl { get; set; }

    public string DefraHelplineEmail { get; set; }

    public string EqualityAdvisorySupportServiceUrl { get; set; }

    public string ContactUsUrl { get; set; }

    public string WebContentAccessibilityUrl { get; set; }

    public string StatementPreparedDate { get; set; }

    public string StatementReviewedDate { get; set; }

    public string SiteTestedDate { get; set; }
}