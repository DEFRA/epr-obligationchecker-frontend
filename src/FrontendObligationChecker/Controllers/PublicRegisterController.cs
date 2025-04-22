namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Exceptions;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.ViewModels.LargeProducer;
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
        public async Task<IActionResult> Get()
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

        [HttpGet(PagePath.Report)]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> File(string fileName, string type)
        {
            try
            {
                if(string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(type))
                {
                    return RedirectToAction("Get");
                }

                var containerName = type == _options.PublicRegisterBlobContainerName ? _options.PublicRegisterBlobContainerName : _options.PublicRegisterCsoBlobContainerName;
                var fileContent = await _blobStorageService.GetLatestFileAsync(containerName);
                if (fileContent == null)
                {
                    return RedirectToAction(nameof(PagePath.FileNotDownloaded));
                }

                return File(fileContent, "text/csv", fileName);
            }
            catch (LargeProducerRegisterServiceException ex)
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }
        }

        [HttpGet(PagePath.FileNotDownloaded)]
        public async Task<IActionResult> FileNotDownloaded()
        {
            return View("GuidanceError", new LargeProducerErrorViewModel());
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