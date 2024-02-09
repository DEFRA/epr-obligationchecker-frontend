namespace FrontendObligationChecker.Services.LargeProducerRegister;

using ByteSizeLib;
using Constants;
using Exceptions;
using Extensions;
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

    public async Task<(Stream Stream, string FileName)> GetReportAsync(string nationCode)
    {
        var fileName = _largeProducerReportFileNamesConfig.GetFileNameFromNationCode(nationCode);
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

    public async Task<Dictionary<string, string>> GetAllReportFileSizesAsync()
    {
        var fileNameMapping = _largeProducerReportFileNamesConfig.GetAllNationCodeToFileNameMappings();

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
}