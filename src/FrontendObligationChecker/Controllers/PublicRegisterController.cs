namespace FrontendObligationChecker.Controllers
{
    using System.Globalization;
    using System.Linq;
    using Constants;
    using Constants.PublicRegister;
    using Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement.Mvc;
    using Models.BlobReader;
    using Models.Config;
    using Services.PublicRegister;
    using Services.PublicRegister.Interfaces;
    using ViewModels.PublicRegister;

    [FeatureGate(FeatureFlags.PublicRegisterEnabled)]
    [Route(PagePath.PublicRegister)]
    public class PublicRegisterController(
        IBlobStorageService blobStorageService,
        IOptions<ExternalUrlsOptions> urlOptions,
        IOptions<EmailAddressOptions> emailAddressOptions,
        IOptions<PublicRegisterOptions> publicRegisterOptions,
        IFeatureFlagService featureFlagService,
        ILogger<PublicRegisterController> logger) : Controller
    {
        private readonly ILogger<PublicRegisterController> _logger = logger;
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly PublicRegisterOptions _options = publicRegisterOptions.Value;
        private readonly ExternalUrlsOptions _urlOptions = urlOptions.Value;
        private readonly EmailAddressOptions _emailAddressOptions = emailAddressOptions.Value;
        private readonly IFeatureFlagService _featureFlagService = featureFlagService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var (isComplianceSchemesRegisterEnabled, isEnforcementActionsSectionEnabled, isPublicRegisterNextYearEnabled) = await GetFeatureFlagsAsync();

            GuidanceViewModel viewModel = await GetRegisterViewModel(
                isComplianceSchemesRegisterEnabled: isComplianceSchemesRegisterEnabled,
                isEnforcementActionsSectionEnabled: isEnforcementActionsSectionEnabled,
                isPublicRegisterNextYearEnabled: isPublicRegisterNextYearEnabled,
                optionsCurrentYear: _options.CurrentYear,
                optionsPublicRegisterPreviousYearEndMonthAndDay: _options.PublicRegisterPreviousYearEndMonthAndDay,
                optionsPublicRegisterNextYearStartMonthAndDay: _options.PublicRegisterNextYearStartMonthAndDay,
                optionsPublishedDate: _options.PublishedDate,
                urlOptionsDefraUrl: _urlOptions.DefraUrl,
                urlOptionsBusinessAndEnvironmentUrl: _urlOptions.BusinessAndEnvironmentUrl,
                defraHelplineEmail: _emailAddressOptions.DefraHelpline,
                urlOptionsPublicRegisterScottishProtectionAgency: _urlOptions.PublicRegisterScottishProtectionAgency,
                getUtcNow: () => string.IsNullOrWhiteSpace(_options.FakeDateTimeUtcNow)
                    ? DateTime.UtcNow
                    : DateTime.ParseExact(_options.FakeDateTimeUtcNow, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                blobStorageService: blobStorageService,
                optionsPublicRegisterBlobContainerName: _options.PublicRegisterBlobContainerName,
                optionsPublicRegisterCsoBlobContainerName: _options.PublicRegisterCsoBlobContainerName);

            return View("Guidance", viewModel);
        }

        public static async Task<GuidanceViewModel> GetRegisterViewModel(
            bool isComplianceSchemesRegisterEnabled,
            bool isEnforcementActionsSectionEnabled,
            bool isPublicRegisterNextYearEnabled,
            string? optionsCurrentYear,
            string? optionsPublicRegisterPreviousYearEndMonthAndDay,
            string optionsPublicRegisterNextYearStartMonthAndDay,
            DateTime optionsPublishedDate,
            string urlOptionsDefraUrl,
            string urlOptionsBusinessAndEnvironmentUrl,
            string defraHelplineEmail,
            string urlOptionsPublicRegisterScottishProtectionAgency,
            Func<DateTime> getUtcNow,
            IBlobStorageService blobStorageService,
            string? optionsPublicRegisterBlobContainerName,
            string? optionsPublicRegisterCsoBlobContainerName)
        {
            int currentYear = string.IsNullOrWhiteSpace(optionsCurrentYear)
                ? getUtcNow().Year
                : int.Parse(optionsCurrentYear);

            int previousYear = currentYear - 1;
            int nextYear = currentYear + 1;
            var folderPrefixes = new List<string> { currentYear.ToString() };

            var endMonthDay = optionsPublicRegisterPreviousYearEndMonthAndDay;
            var currentMonthDay = getUtcNow().ToString("MM-dd");

            // Add previous year if today is on or before the configured month and day
            if (!string.IsNullOrWhiteSpace(endMonthDay) &&
                 string.Compare(currentMonthDay, endMonthDay, StringComparison.Ordinal) <= 0)
            {
                folderPrefixes.Add(previousYear.ToString());
            }

            // Add next year if feature enabled and today is on or after the configured month and day
            if (isPublicRegisterNextYearEnabled)
            {
                var startMonthDay = optionsPublicRegisterNextYearStartMonthAndDay;
                if (!string.IsNullOrWhiteSpace(startMonthDay) &&
                    string.Compare(currentMonthDay, startMonthDay, StringComparison.Ordinal) >= 0)
                {
                    folderPrefixes.Add(nextYear.ToString());
                }
            }

            // dictionary key is the year, e.g. "2025"
            var producerBlobModels = await blobStorageService
                .GetLatestFilePropertiesAsync(optionsPublicRegisterBlobContainerName, folderPrefixes);

            // Determine which year to display as "current"
            PublicRegisterBlobModel? producerBlobModelCurrentYear = null;
            PublicRegisterBlobModel? producerBlobModelNextYear = null;

            // Prefer previous year if available and within display window
            if (producerBlobModels.TryGetValue(previousYear.ToString(), out var previousYearBlob) &&
                folderPrefixes.Contains(previousYear.ToString()))
            {
                producerBlobModelCurrentYear = previousYearBlob;
                currentYear = previousYear;
                nextYear = currentYear + 1;
            }
            else
            {
                producerBlobModels.TryGetValue(currentYear.ToString(), out producerBlobModelCurrentYear);
            }

            // Next year logic
            producerBlobModels.TryGetValue(nextYear.ToString(), out producerBlobModelNextYear);

            var publishedDate = FormatDate(optionsPublishedDate);

#pragma warning disable S2589
            DateTime lastUpdated = producerBlobModels?.Values?
                .Where(x => x?.LastModified.HasValue == true)
                .Select(x => x!.LastModified!.Value)
                .DefaultIfEmpty(optionsPublishedDate)
                .Max() ?? optionsPublishedDate;
#pragma warning restore S2589

            var lastUpdatedFormatted = FormatDate(lastUpdated);

            var viewModel = new GuidanceViewModel
            {
                DefraUrl = urlOptionsDefraUrl,
                BusinessAndEnvironmentUrl = urlOptionsBusinessAndEnvironmentUrl,
                DefraHelplineEmail = defraHelplineEmail,
                PublishedDate = publishedDate,
                Currentyear = currentYear.ToString(),
                Nextyear = nextYear.ToString(),
                LastUpdated = lastUpdatedFormatted,
                ProducerRegisteredFile = MapToFileViewModel(producerBlobModelCurrentYear, publishedDate, lastUpdatedFormatted),
                ProducerRegisteredFileNextYear = producerBlobModelNextYear != null ? MapToFileViewModel(producerBlobModelNextYear, publishedDate, lastUpdatedFormatted) : null
            };

            if (isComplianceSchemesRegisterEnabled)
            {
                var complianceBlobModel = await blobStorageService
                    .GetLatestFilePropertiesAsync(optionsPublicRegisterCsoBlobContainerName);

                viewModel.ComplianceSchemeRegisteredFile = MapToFileViewModel(complianceBlobModel, publishedDate, lastUpdatedFormatted);
            }

            if (isEnforcementActionsSectionEnabled)
            {
                var enforcementActionFiles = await blobStorageService.GetEnforcementActionFiles();

                viewModel.EnforcementActionFiles = enforcementActionFiles;

                viewModel.EnglishEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.England);
                viewModel.WelshEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.Wales);
                viewModel.NortherIrishEnforcementActionFile = GetEnforcementActionFileViewModel(enforcementActionFiles, EnforcementAgency.NorthernIreland);

                viewModel.ScottishEnforcementActionFileUrl = urlOptionsPublicRegisterScottishProtectionAgency;
            }

            return viewModel;
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
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["ReportFileName"] = fileName, ["ReportType"] = type
            });

            try
            {
                _logger.LogInformation("Downloading public register of producers");

                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(type))
                {
                    _logger.LogWarning("File name or file type not specified");
                    return RedirectToAction(nameof(PagePath.FileNotDownloaded));
                }

                var fileModel = await _blobStorageService.GetLatestFileAsync(type, fileName);

                if (string.IsNullOrWhiteSpace(fileModel.FileName))
                {
                    _logger.LogWarning("File not found in blob storage");
                    return RedirectToAction(nameof(PagePath.FileNotDownloaded));
                }

                return File(fileModel.FileContent, "text/csv", fileModel.FileName);
            }
            catch (PublicRegisterServiceException ex)
            {
                _logger.LogError(ex, "Caught exception downloading file");
                return RedirectToAction(nameof(PagePath.FileNotDownloaded));
            }
            finally
            {
                _logger.LogInformation("Download of public register of producers complete");
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
                FileName = blobModel?.Name,
                FileSize = blobModel?.ContentLength?.ToString() ?? "0",
                FileType = blobModel?.FileType ?? "CSV"
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