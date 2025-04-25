namespace FrontendObligationChecker.Services.PublicRegister;

using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FrontendObligationChecker.Exceptions;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Readers;
using FrontendObligationChecker.ViewModels.PublicRegister;
using Microsoft.Extensions.Options;

public class BlobStorageService(
    BlobServiceClient blobServiceClient,
    IBlobReader blobReader,
    ILogger<BlobStorageService> logger,
    IOptions<PublicRegisterOptions> publicRegisterOptions) : IBlobStorageService
{
    private const string ErrorMessage = "Failed to read {0} from blob storage";
    private const string LogMessage = "Failed to read {FileName} from blob storage";

    public async Task<PublicRegisterBlobModel?> GetLatestFilePropertiesAsync(string containerName)
    {
        var result = new PublicRegisterBlobModel
        {
            PublishedDate = publicRegisterOptions.Value.PublishedDate
        };

        try
        {
            var containerClient = GetContainerClient(containerName);
            if (containerClient is null) return result;

            var latestFolderPrefix = await GetLatestFolderPrefixAsync(containerClient);
            if (string.IsNullOrWhiteSpace(latestFolderPrefix)) return result;

            var latestBlob = await GetLatestBlobAsync(containerClient, latestFolderPrefix);
            if (latestBlob is null) return result;

            var blobClient = containerClient.GetBlobClient(latestBlob.Name);
            var properties = await blobClient.GetPropertiesAsync();

            result.Name = latestBlob.Name;
            result.LastModified = properties.Value.LastModified.DateTime;
            result.ContentLength = properties.Value.ContentLength.ToString();
            result.FileType = GetFileType(latestBlob.Name);

            return result;
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, LogMessage, $"{containerName} files");
        }

        return result;
    }

    public async Task<EnforcementActionFileViewModel> GetEnforcementActionFileByHomeNation(string homeNation)
    {
        var result = new EnforcementActionFileViewModel();
        var containerName = publicRegisterOptions.Value.EnforcementActionsBlobContainerName;

        try
        {
            var containerClient = GetContainerClient(containerName);

            if (containerClient is null) return result;

            // Used as a filter.
            var suffix = string.Format("_{0}", homeNation);

            var enforcementFileName = string.Format("{0}{1}.{2}", publicRegisterOptions.Value.EnforcementActionFileName, suffix, "xlsx");

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);

                if (blobItem.Name == enforcementFileName)
                {
                    result.FileName = blobItem.Name;
                    result.ContentFileLength = (int)blobItem.Properties.ContentLength;
                    result.DateCreated = blobItem.Properties.CreatedOn.Value.DateTime;
                    result.FileContents = await blobReader.DownloadBlobToStreamAsync(containerName, blobItem.Name, true);

                    break;
                }
            }
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, LogMessage, $"{containerName} files");
        }

        return result;
    }

    private static string? GetFolderPrefix(string blobName)
    {
        var parts = blobName.Split('/');
        return parts.Length > 1 ? parts[0] + "/" : null;
    }

    private static string GetFileType(string blobName)
    {
        var extension = Path.GetExtension(blobName);
        return string.IsNullOrWhiteSpace(extension) ? "CSV" : extension.TrimStart('.').ToUpperInvariant();
    }

    private static async Task<BlobItem?> GetLatestBlobAsync(BlobContainerClient containerClient, string prefix)
    {
        BlobItem? latestBlob = null;

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            if (latestBlob is null || blobItem.Properties?.LastModified > latestBlob.Properties?.LastModified)
            {
                latestBlob = blobItem;
            }
        }

        return latestBlob;
    }

    private BlobContainerClient GetContainerClient(string containerName)
    {
        return blobServiceClient.GetBlobContainerClient(containerName);
    }

    private async Task<string?> GetLatestFolderPrefixAsync(BlobContainerClient containerClient)
    {
        try
        {
            var folderPrefixes = new HashSet<string>();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                var prefix = GetFolderPrefix(blobItem.Name);
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    folderPrefixes.Add(prefix);
                }
            }

            return folderPrefixes
                .OrderByDescending(p => p)
                .FirstOrDefault();
        }
        catch (RequestFailedException ex)
        {
            LogError(ex, "directories");
            throw new BlobReaderException(string.Format(ErrorMessage, "directories"), ex);
        }
    }

    private void LogError(RequestFailedException ex, string fileName)
    {
        logger.LogError(ex, LogMessage, fileName);
    }

    public async Task<IEnumerable<EnforcementActionFileViewModel>> GetEnforcementActionFiles()
    {
        var results = new List<EnforcementActionFileViewModel>();

        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(publicRegisterOptions.Value.EnforcementActionsBlobContainerName);
            if (containerClient is null)
            {
                return results;
            }

            await foreach(BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                var enforcementActionFileItem = new EnforcementActionFileViewModel();

                enforcementActionFileItem.FileName = blobItem.Name;
                enforcementActionFileItem.DateCreated = DateTime.Now;
                enforcementActionFileItem.ContentFileLength = (int)blobItem.Properties.ContentLength;

                results.Add(enforcementActionFileItem);
            }

        }
        catch (RequestFailedException ex)
        {
            LogError(ex, "directories");
            throw new BlobReaderException(string.Format(ErrorMessage, "directories"), ex);
        }

        return results;
    }
}