namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController(
        IBlobStorageService blobStorageService,
        IOptions<ExternalUrlsOptions> urlOptions,
        IOptions<PublicRegisterOptions> publicRegisterOptions) : Controller
    {
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly PublicRegisterOptions _options = publicRegisterOptions.Value;

        [HttpGet]
        public async Task<IActionResult> Guidance()
        {
            var producerBlobModel = await _blobStorageService
                .GetLatestFilePropertiesAsync(_options.PublicRegisterBlobContainerName);

            var complianceBlobModel = await _blobStorageService
                .GetLatestFilePropertiesAsync(_options.PublicRegisterCsoBlobContainerName);

            var publishedDate = FormatDate(producerBlobModel.PublishedDate);
            var lastUpdated = FormatDate(producerBlobModel.LastModified) ?? publishedDate;

            var viewModel = new GuidanceViewModel
            {
                DefraUrl = urlOptions.Value.DefraUrl,
                PublishedDate = publishedDate,
                LastUpdated = lastUpdated,
                ProducerRegisteredFile = MapToFileViewModel(producerBlobModel, publishedDate, lastUpdated),
                ComplianceSchemeRegisteredFile = MapToFileViewModel(complianceBlobModel, publishedDate, lastUpdated)
            };

            return View("Guidance", viewModel);
        }

        private static string FormatDate(DateTime? date) =>
            date?.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);

        private static PublicRegisterFileViewModel MapToFileViewModel(PublicRegisterBlobModel blobModel, string publishedDate, string lastUpdated)
        {
            return new PublicRegisterFileViewModel
            {
                DatePublished = publishedDate,
                DateLastModified = lastUpdated,
                FileName = blobModel.Name,
                FileSize = blobModel.ContentLength?.ToString() ?? "0",
                FileType = blobModel.FileType
            };
        }
    }
}