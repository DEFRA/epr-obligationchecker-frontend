namespace FrontendObligationChecker.Readers;

using Azure;
using Azure.Storage.Blobs;
using Exceptions;

public class BlobReader : IBlobReader
{
    private const string ErrorMessage = "Failed to read {0} from blob storage";
    private const string LogMessage = "Failed to read {FileName} from blob storage";

    private readonly BlobContainerClient _blobContainerClient;
    private readonly ILogger<BlobReader> _logger;

    public BlobReader(BlobContainerClient blobContainerClient, ILogger<BlobReader> logger)
    {
        _blobContainerClient = blobContainerClient;
        _logger = logger;
    }

    public async Task<Stream> DownloadBlobToStreamAsync(string fileName)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, LogMessage, fileName);
            throw new BlobReaderException(string.Format(ErrorMessage, fileName), ex);
        }
    }

    public async Task<long> GetFileSizeInBytesAsync(string fileName)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            var blobProperties = await blobClient.GetPropertiesAsync();
            return blobProperties.Value.ContentLength;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, LogMessage, fileName);
            throw new BlobReaderException(string.Format(ErrorMessage, fileName), ex);
        }
    }
}