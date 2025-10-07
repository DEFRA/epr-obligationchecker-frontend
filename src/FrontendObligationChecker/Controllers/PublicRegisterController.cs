namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using System.Linq;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Constants.PublicRegister;
    using FrontendObligationChecker.Exceptions;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.Services.PublicRegister.Interfaces;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController(
        IBlobStorageService blobStorageService,
        IOptions<ExternalUrlsOptions> urlOptions,
        IOptions<EmailAddressOptions> emailAddressOptions,
        IOptions<PublicRegisterOptions> publicRegisterOptions,
        IFeatureFlagService featureFlagService) : Controller
    {
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly PublicRegisterOptions _options = publicRegisterOptions.Value;
        private readonly ExternalUrlsOptions _urlOptions = urlOptions.Value;
        private readonly EmailAddressOptions _emailAddressOptions = emailAddressOptions.Value;
        private readonly IFeatureFlagService _featureFlagService = featureFlagService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var (isComplianceSchemesRegisterEnabled, isEnforcementActionsSectionEnabled, isPublicRegisterNextYearEnabled) = await GetFeatureFlagsAsync();

            var currentYear = DateTime.UtcNow.Year;
            var nextYear = currentYear + 1;
            var folderPrefixes = new List<string> { currentYear.ToString() };

            if (isPublicRegisterNextYearEnabled)
            {
                var startMonthDay = _options.PublicRegisterNextYearStartMonthAndDay;
                var currentYearMonthDay = DateTime.UtcNow.ToString("MM-dd");

                // If today is on or after the configured month and day, the next year folder is added.
                if (string.Compare(currentYearMonthDay, startMonthDay, StringComparison.Ordinal) >= 0)
                {
                    folderPrefixes.Add(nextYear.ToString());
                }
            }

            var producerBlobModels = await _blobStorageService
                .GetLatestFilePropertiesAsync(_options.PublicRegisterBlobContainerName, folderPrefixes);

            producerBlobModels.TryGetValue(currentYear.ToString(), out var producerBlobModelCurrentYear);
            producerBlobModels.TryGetValue(nextYear.ToString(), out var producerBlobModelNextYear);

            var publishedDate = FormatDate(publicRegisterOptions.Value.PublishedDate);

            DateTime lastUpdated;

            lastUpdated = producerBlobModels.Values
                                        .Where(x => x?.LastModified.HasValue == true)
                                        .Select(x => x.LastModified.Value)
                                        .DefaultIfEmpty(_options.PublishedDate)
                                        .Max();

            var lastUpdatedFormatted = FormatDate(lastUpdated);

            var viewModel = new GuidanceViewModel
            {
                DefraUrl = _urlOptions.DefraUrl,
                BusinessAndEnvironmentUrl = _urlOptions.BusinessAndEnvironmentUrl,
                DefraHelplineEmail = _emailAddressOptions.DefraHelpline,
                PublishedDate = publishedDate,
                Currentyear = currentYear.ToString(),
                Nextyear = nextYear.ToString(),
                LastUpdated = lastUpdatedFormatted,
                ProducerRegisteredFile = producerBlobModelCurrentYear != null ? MapToFileViewModel(producerBlobModelCurrentYear, publishedDate, lastUpdatedFormatted) : null,
                ProducerRegisteredFileNextYear = producerBlobModelNextYear != null ? MapToFileViewModel(producerBlobModelNextYear, publishedDate, lastUpdatedFormatted) : null
            };

            if (isComplianceSchemesRegisterEnabled)
            {
                var complianceBlobModel = await _blobStorageService
                    .GetLatestFilePropertiesAsync(_options.PublicRegisterCsoBlobContainerName);

                viewModel.ComplianceSchemeRegisteredFile = MapToFileViewModel(complianceBlobModel, publishedDate, lastUpdatedFormatted);
            }

            if (isEnforcementActionsSectionEnabled)
            {
                var enforcementActionFiles = await _blobStorageService.GetEnforcementActionFiles();
                viewModel.EnforcementActionFiles = enforcementActionFiles;

                viewModel.EnglishEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.England);
                viewModel.WelshEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.Wales);
                viewModel.NortherIrishEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.NorthernIreland);

                viewModel.ScottishEnforcementActionFileUrl = _urlOptions.PublicRegisterScottishProtectionAgency;
            }

            return View("Guidance", viewModel);
        }

        [HttpGet(PagePath.Enforce)]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> File(string agency)
        {
            if (string.IsNullOrWhiteSpace(agency))
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }

            try
            {
                var latestFile = await _blobStorageService.GetEnforcementActionFileByAgency(agency);

                if (latestFile is null || latestFile.FileContents is null)
                {
                    return RedirectToAction(nameof(PagePath.FileNotDownloaded));
                }

                return File(latestFile.FileContents, "text/csv", latestFile.FileName);
            }
            catch (LargeProducerRegisterServiceException ex)
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }
        }

        [HttpGet(PagePath.Report)]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> File(string fileName, string type)
        {
            try
            {
                var fileModel = await _blobStorageService.GetLatestFileAsync(type, fileName);
                if (string.IsNullOrEmpty(fileModel.FileName))
                {
                    return RedirectToAction(nameof(PagePath.FileNotDownloaded));
                }

                return File(fileModel.FileContent, "text/csv", fileModel.FileName);
            }
            catch (PublicRegisterServiceException ex)
            {
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }
        }

        [HttpGet(PagePath.FileNotDownloaded)]
        public async Task<IActionResult> FileNotDownloaded()
        {
            return View("GuidanceError", new PublicRegisterErrorViewModel());
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

        private static EnforcementActionFileViewModel? GetEnforcementActionFileViewModel(IEnumerable<EnforcementActionFileViewModel> files, string agency)
        {
            var file = files.FirstOrDefault(x => x.FileName.Contains($"_{agency}"));

            if (file is not null)
            {
                file.FileDownloadUrl = $"/public-register/enforce?&agency={agency}";
            }

            return file;
        }

        private async Task<(bool ComplianceEnabled, bool EnforcementEnabled, bool NextYearEnabled)> GetFeatureFlagsAsync()
        {
            var compliance = await _featureFlagService.IsComplianceSchemesRegisterEnabledAsync();
            var enforcement = await _featureFlagService.IsEnforcementActionsSectionEnabledAsync();
            var nextYear = await _featureFlagService.IsPublicRegisterNextYearEnabledAsync();
            return (compliance, enforcement, nextYear);
        }
    }
}