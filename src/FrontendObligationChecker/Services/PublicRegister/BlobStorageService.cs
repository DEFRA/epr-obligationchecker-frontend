namespace FrontendObligationChecker.Services.PublicRegister;

using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FrontendObligationChecker.Exceptions;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Models.Config;
using Microsoft.Extensions.Options;

public class BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger, IOptions<PublicRegisterOptions> publicRegisterOptions) : IBlobStorageService
{
    private const string ErrorMessage = "Failed to read {0} from blob storage";
    private const string LogMessage = "Failed to read {FileName} from blob storage";

    public async Task<PublicRegisterBlobModel?> GetLatestProducersFilePropertiesAsync()
    {
        var result = new PublicRegisterBlobModel
        {
            PublishedDate = publicRegisterOptions.Value.PublishedDate
        };

        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(publicRegisterOptions.Value.PublicRegisterBlobContainerName);
            if (containerClient == null) return result;

            BlobItem? latestBlob = null;
            string? latestFolderPrefix = null;

            latestFolderPrefix = await LatestFolder(containerClient, latestFolderPrefix);

            if (string.IsNullOrEmpty(latestFolderPrefix)) return result;

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: latestFolderPrefix))
            {
                if (latestBlob == null || blobItem.Properties?.LastModified > latestBlob.Properties?.LastModified)
                {
                    latestBlob = blobItem;
                }
            }

            if (latestBlob == null) return result;

            var blobClient = containerClient.GetBlobClient(latestBlob.Name);
            var properties = await blobClient.GetPropertiesAsync();

            result.LastModified = properties.Value.LastModified.DateTime;
            result.ContentLength = properties.Value.ContentLength.ToString();
            result.Name = latestBlob.Name;
            return result;
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, LogMessage, "Producers files");
        }

        return result;
    }

    private async Task<string?> LatestFolder(BlobContainerClient containerClient, string? latestFolderPrefix)
    {
        try
        {
            var blobs = new List<BlobItem>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(blobItem);
            }

            latestFolderPrefix = blobs
                .Where(blob => blob.Name.Contains('/'))
                .Select(blob => blob.Name.Split('/')[0] + "/")
                .OrderDescending()
                .FirstOrDefault();

            return latestFolderPrefix;
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, LogMessage, "directories");
            throw new BlobReaderException(string.Format(ErrorMessage, "directories"), ex);
        }
    }
}