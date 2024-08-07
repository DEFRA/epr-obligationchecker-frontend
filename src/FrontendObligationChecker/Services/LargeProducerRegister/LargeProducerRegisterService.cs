namespace FrontendObligationChecker.Services.LargeProducerRegister;

using ByteSizeLib;
using Constants;
using Exceptions;
using Extensions;
using FrontendObligationChecker.Models.BlobReader;
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
    private readonly ILogger<LargeProducerRegisterService> _logger;

    public LargeProducerRegisterService(
        IBlobReader blobReader,
        IOptions<LargeProducerReportFileNamesOptions> producerReportFileNamesConfig,
        ILogger<LargeProducerRegisterService> logger)
    {
        _blobReader = blobReader;
        _largeProducerReportFileNamesConfig = producerReportFileNamesConfig.Value;
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

    public async Task<LargeProducerFileInfoViewModel> GetLatestAllNationsFileInfoAsync(string culture)
    {
        var prefix = culture == Language.English
            ? _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefix
            : _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefixInWelsh;

        var latestBlob = await GetLatestBlobAsync(prefix);

        if (latestBlob == null)
        {
            _logger.LogError("Latest blob for culture {culture} not found", culture);
            return null;
        }

        return new LargeProducerFileInfoViewModel
        {
            DateCreated = latestBlob.CreatedOn.Value.Date,
            DisplayFileSize = FileSizeFormatterHelper.ConvertByteSizeToString(ByteSize.FromBytes(latestBlob.ContentLength.Value))
        };
    }

    public async Task<LargeProducerFileViewModel> GetLatestAllNationsFileAsync(string culture)
    {
        var prefix = culture == Language.English
            ? _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefix
            : _largeProducerReportFileNamesConfig.LatestAllNationsReportFileNamePrefixInWelsh;

        var latestBlob = await GetLatestBlobAsync(prefix);

        if (latestBlob == null)
        {
            _logger.LogError("Latest blob for culture {culture} not found", culture);
            return null;
        }

        var downloadFileNamePattern = culture == Language.English
            ? _largeProducerReportFileNamesConfig.LatestAllNationsReportDownloadFileName
            : _largeProducerReportFileNamesConfig.LatestAllNationsReportDownloadFileNameInWelsh;

        try
        {
            return new LargeProducerFileViewModel
            {
                FileName = string.Format(downloadFileNamePattern, latestBlob.CreatedOn.Value.Date),
                FileContents = await _blobReader.DownloadBlobToStreamAsync(latestBlob.Name)
            };
        }
        catch (BlobReaderException ex)
        {
            _logger.LogError(ex, LogMessage, string.Empty);
            throw new LargeProducerRegisterServiceException(string.Format(ErrorMessage, HomeNation.All), ex);
        }
    }

    private async Task<BlobModel> GetLatestBlobAsync(string prefix)
    {
        return (await _blobReader.GetBlobsAsync(prefix))
            .Where(x => x.CreatedOn != null && x.ContentLength != null && x.Name != null)
            .MaxBy(x => x.CreatedOn);
    }
}