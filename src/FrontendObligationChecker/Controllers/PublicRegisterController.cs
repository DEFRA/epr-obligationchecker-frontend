namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController(IOptions<ExternalUrlsOptions> urlOptions) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Guidance()
        {
            // This is hard-coded for the sake of displaying the view for story #523624
            var viewModel = new GuidanceViewModel
            {
                ComplianceSchemesRegisteredFileSize = "450",
                LastUpdated = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc).ToString("d MMMM yyyy", CultureInfo.InvariantCulture),
                ProducersRegisteredFileSize = "115",
                PublishedDate = new DateTime(2025, 12, 6, 0, 0, 0, DateTimeKind.Utc).ToString("d MMMM yyyy", CultureInfo.InvariantCulture),
                DefraUrl = urlOptions.Value.DefraUrl
            };

            return View("Guidance", viewModel);
        }
    }
}