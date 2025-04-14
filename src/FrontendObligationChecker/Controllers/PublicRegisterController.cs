namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController : Controller
    {
        private IBlobStorageService blobStorageService;

        public PublicRegisterController(IBlobStorageService blobStorageService)
        {
            this.blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Guidance()
        {
            PublicRegisterBlobModel blobModel = await blobStorageService.GetLatestProducersFilePropertiesAsync();

            string publishedDate = blobModel.PublishedDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
            string lastUpdated = blobModel.LastModified?.ToString("d MMMM yyyy", CultureInfo.InvariantCulture) ?? publishedDate;
            string producersRegisteredFileSize = blobModel.ContentLength?.ToString() ?? "0";

            // This is hard-coded cso data for the sake of displaying the view for story #523624
            var viewModel = new GuidanceViewModel
            {
                PublishedDate = publishedDate,
                LastUpdated = lastUpdated,
                ProducersRegisteredFileSize = producersRegisteredFileSize,
                ComplianceSchemesRegisteredFileSize = "450"
            };

            return View("Guidance", viewModel);
        }
    }
}