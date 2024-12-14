namespace FrontendObligationChecker.Services.LargeProducerRegister;

using System.Linq;
using ByteSizeLib;
using Constants;
using Exceptions;
using Extensions;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Services.Caching;
using FrontendObligationChecker.ViewModels.LargeProducer;
using Helpers;
using Interfaces;
using Microsoft.Extensions.Options;
using Models.Config;
using Readers;

public class LargeProducerRegisterService : ILargeProducerRegisterService
{
    private const string ErrorMessage = "Failed to get report for nation code {0}";
    private const string LogMessage = "Failed to get report for nation code {NationCode}";
    private const string LogMessageMetadata = "Failed to get report metadata";

    private readonly IBlobReader _blobReader;
    private readonly LargeProducerReportFileNamesOptions _largeProducerReportFileNamesConfig;
    private readonly ICacheService _cacheService;
    private readonly ILogger<LargeProducerRegisterService> _logger;

    public LargeProducerRegisterService(
        IBlobReader blobReader,
        IOptions<LargeProducerReportFileNamesOptions> producerReportFileNamesConfig,
        ICacheService cacheService,
        ILogger<LargeProducerRegisterService> logger)
    {
        _blobReader = blobReader;
        _largeProducerReportFileNamesConfig = producerReportFileNamesConfig.Value;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<(Stream Stream, string FileName)> GetReportAsync(string nationCode, string culture)
    {
        var fileName = _largeProducerReportFileNamesConfig.GetFileNameFromNationCodeAndCulture(nationCode, culture);
        try
        {
            return (await _blobReader.DownloadBlobToStreamAsync(fileName), fileName);
        }
        catch (BlobReaderException ex)
        {
            _logger.LogError(ex, LogMessage, nationCode);
            throw new LargeProducerRegisterServiceException(string.Format(ErrorMessage, nationCode), ex);
        }
    }

    public async Task<Dictionary<string, string>> GetAllReportFileSizesAsync(string culture)
    {
        var fileNameMapping = _largeProducerReportFileNamesConfig.GetAllNationCodeToFileNameMappings(culture);

        try
        {
            return new Dictionary<string, string>()
            {
                {
                    HomeNation.England, FileSizeFormatterHelper.ConvertByteSizeToString(
                        ByteSize.FromBytes(
                            await _blobReader.GetFileSizeInBytesAsync(fileNameMapping[HomeNation.England])))
                },
                {
                    HomeNation.Scotland, FileSizeFormatterHelper.ConvertByteSizeToString(
                        ByteSize.FromBytes(
                            await _blobReader.GetFileSizeInBytesAsync(fileNameMapping[HomeNation.Scotland])))
                },
                {
                    HomeNation.Wales, FileSizeFormatterHelper.ConvertByteSizeToString(
                        ByteSize.FromBytes(
                            await _blobReader.GetFileSizeInBytesAsync(fileNameMapping[HomeNation.Wales])))
                },
                {
                    HomeNation.NorthernIreland, FileSizeFormatterHelper.ConvertByteSizeToString(
                        ByteSize.FromBytes(
                            await _blobReader.GetFileSizeInBytesAsync(fileNameMapping[HomeNation.NorthernIreland])))
                },
                {
                    HomeNation.All, FileSizeFormatterHelper.ConvertByteSizeToString(
                        ByteSize.FromBytes(
                            await _blobReader.GetFileSizeInBytesAsync(fileNameMapping[HomeNation.All])))
                }
            };
        }
        catch (BlobReaderException ex)
        {
            _logger.LogError(ex, LogMessageMetadata);
            throw new LargeProducerRegisterServiceException(LogMessageMetadata, ex);
        }
    }

    public async Task<IEnumerable<LargeProducerFileInfoViewModel>> GetLatestAllNationsFileInfoAsync(string culture)
    {
        var filenamePrefix = culture == Language.English
            ? _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefix
            : _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefixInWelsh;

        var latestFiles = new List<LargeProducerFileInfoViewModel>();

        foreach (var reportingYearDirectory in await GetReportDirectories())
        {
            var prefix = $"{reportingYearDirectory}/{filenamePrefix}";

            var latestBlob = await GetLatestBlobAsync(prefix);

            if (latestBlob != null)
            {
                latestFiles.Add(new LargeProducerFileInfoViewModel
                {
                    ReportingYear = int.Parse(reportingYearDirectory),
                    DateCreated = latestBlob.CreatedOn.Value.Date,
                    DisplayFileSize = FileSizeFormatterHelper.ConvertByteSizeToString(ByteSize.FromBytes(latestBlob.ContentLength.Value))
                });
            }
            else
            {
                _logger.LogError("Latest blob with {Prefix} prefix not found", prefix);
            }
        }

        return latestFiles;
    }

    public async Task<LargeProducerFileViewModel> GetLatestAllNationsFileAsync(int reportingYear, string culture)
    {
        string filenamePrefix;
        string downloadFileNamePattern;

        if (culture == Language.English)
        {
            filenamePrefix = _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefix;
            downloadFileNamePattern = _largeProducerReportFileNamesConfig.LatestAllNationsReportDownloadFileName;
        }
        else
        {
            filenamePrefix = _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefixInWelsh;
            downloadFileNamePattern = _largeProducerReportFileNamesConfig.LatestAllNationsReportDownloadFileNameInWelsh;
        }

        var latestBlob = await GetLatestBlobAsync($"{reportingYear}/{filenamePrefix}");

        if (latestBlob == null)
        {
            _logger.LogError("Latest blob for reporting year {ReportingYear} and culture {Culture} not found", reportingYear, culture);
            return null;
        }

        try
        {
            return new LargeProducerFileViewModel
            {
                FileName = string.Format(downloadFileNamePattern, reportingYear, latestBlob.CreatedOn.Value.Date),
                FileContents = await _blobReader.DownloadBlobToStreamAsync(latestBlob.Name, true)
            };
        }
        catch (BlobReaderException ex)
        {
            _logger.LogError(ex, LogMessage, HomeNation.All);
            throw new LargeProducerRegisterServiceException(string.Format(ErrorMessage, HomeNation.All), ex);
        }
    }

    private async Task<BlobModel> GetLatestBlobAsync(string prefix)
    {
        if (_cacheService.GetBlobModelCache(prefix, out BlobModel blobModel))
        {
            return blobModel;
        }

        blobModel = (await _blobReader.GetBlobsAsync(prefix))
            .Where(x => x.CreatedOn != null && x.ContentLength != null && x.Name != null)
            .MaxBy(x => x.CreatedOn);

        if (blobModel != null)
        {
            _cacheService.SetBlobModelCache(prefix, blobModel);
        }

        return blobModel;
    }

    private async Task<IEnumerable<string>> GetReportDirectories()
    {
        if (_cacheService.GetReportDirectoriesCache(out IEnumerable<string> reportDirectories))
        {
            return reportDirectories;
        }

        reportDirectories = (await _blobReader.GetDirectories())
            .Select(x => x.TrimEnd('/'))
            .OrderDescending()
            .ToArray();

        if (reportDirectories.Any())
        {
            _cacheService.SetReportDirectoriesCache(reportDirectories);
        }

        return reportDirectories;
    }
}