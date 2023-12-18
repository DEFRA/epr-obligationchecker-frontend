using FrontendObligationChecker.Extensions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrontendObligationChecker.Controllers;

public class PrivacyController : Controller
{
    private readonly ILogger<PrivacyController> _logger;
    private readonly ExternalUrlsOptions _urlOptions;
    private readonly EmailAddressOptions _emailOptions;
    private readonly SiteDateOptions _siteDateOptions;

    public PrivacyController(
        ILogger<PrivacyController> logger,
        IOptions<ExternalUrlsOptions> urlOptions,
        IOptions<EmailAddressOptions> emailOptions,
        IOptions<SiteDateOptions> siteDateOptions)
    {
        _logger = logger;
        _urlOptions = urlOptions?.Value;
        _emailOptions = emailOptions?.Value;
        _siteDateOptions = siteDateOptions?.Value;
    }

    [HttpGet]
    [Route("privacy")]
    public async Task<IActionResult> Detail(string returnUrl)
    {
        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = Url.HomePath();
        }

        var model = new PrivacyViewModel
        {
            CurrentPage = returnUrl,
            BackLinkToDisplay = Url.Content(returnUrl),
            DataProtectionPublicRegisterUrl = _urlOptions.PrivacyDataProtectionPublicRegister,
            WebBrowserUrl = _urlOptions.PrivacyWebBrowser,
            GoogleAnalyticsUrl = _urlOptions.PrivacyGoogleAnalytics,
            EuropeanEconomicAreaUrl = _urlOptions.PrivacyEuropeanEconomicArea,
            DefrasPersonalInformationCharterUrl = _urlOptions.PrivacyDefrasPersonalInformationCharter,
            InformationCommissionerUrl = _urlOptions.PrivacyInformationCommissioner,
            FindOutAboutCallChargesUrl = _urlOptions.PrivacyFindOutAboutCallCharges,
            DataProtectionEmail = _emailOptions.DataProtection,
            InformationCommissionerEmail = _emailOptions.InformationCommissioner,
            DefraGroupProtectionOfficerEmail = _emailOptions.DefraGroupProtectionOfficer,
            LastUpdated = _siteDateOptions.PrivacyLastUpdated.ToString(_siteDateOptions.DateFormat)
        };

        return View(model);
    }
}