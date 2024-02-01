namespace FrontendObligationChecker.Controllers;

using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Config;
using Models.LargeProducerRegister;
using ViewModels;

[Route(PagePath.Accessibility)]
public class AccessibilityController : Controller
{
    private readonly ExternalUrlsOptions _urlOptions;
    private readonly EmailAddressOptions _emailOptions;
    private readonly SiteDateOptions _siteDateOptions;

    public AccessibilityController(
        IOptions<ExternalUrlsOptions> urlOptions,
        IOptions<EmailAddressOptions> emailOptions,
        IOptions<SiteDateOptions> siteDateOptions)
    {
        _urlOptions = urlOptions?.Value;
        _emailOptions = emailOptions?.Value;
        _siteDateOptions = siteDateOptions?.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string returnUrl)
    {
        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = Url.HomePath();
        }

        string dateFormat = _siteDateOptions.DateFormat;

        var model = new AccessibilityViewModel
        {
            CurrentPage = returnUrl,
            BackLinkToDisplay = Url.Content(returnUrl),
            AbilityNetUrl = _urlOptions.AccessibilityAbilityNet,
            ContactUsUrl = _urlOptions.AccessibilityContactUs,
            EqualityAdvisorySupportServiceUrl = _urlOptions.AccessibilityEqualityAdvisorySupportService,
            WebContentAccessibilityUrl = _urlOptions.AccessibilityWebContentAccessibility,
            DefraHelplineEmail = _emailOptions.DefraHelpline,
            SiteTestedDate = _siteDateOptions.AccessibilitySiteTested.ToString(dateFormat),
            StatementPreparedDate = _siteDateOptions.AccessibilityStatementPrepared.ToString(dateFormat),
            StatementReviewedDate = _siteDateOptions.AccessibilityStatementReviewed.ToString(dateFormat)
        };

        return View(model);
    }
}