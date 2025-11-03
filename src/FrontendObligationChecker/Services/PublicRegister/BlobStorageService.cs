namespace FrontendObligationChecker.Services.PublicRegister;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FrontendObligationChecker.Constants;
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

    public async Task<PublicRegisterBlobModel> GetLatestFilePropertiesAsync(string containerName)
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

    public async Task<Dictionary<string, PublicRegisterBlobModel>> GetLatestFilePropertiesAsync(string containerName, List<string> folderPrefixes)
    {
        var result = new Dictionary<string, PublicRegisterBlobModel>();

        try
        {
            var containerClient = GetContainerClient(containerName);
            if (containerClient is null) return result;

            foreach (var folderPrefix in folderPrefixes)
            {
                if (string.IsNullOrWhiteSpace(folderPrefix)) continue;

                var latestBlob = await GetLatestBlobAsync(containerClient, folderPrefix);
                if (latestBlob is null) continue;

                var blobClient = containerClient.GetBlobClient(latestBlob.Name);
                var properties = await blobClient.GetPropertiesAsync();

                var model = new PublicRegisterBlobModel
                {
                    PublishedDate = publicRegisterOptions.Value.PublishedDate,
                    Name = latestBlob.Name,
                    LastModified = properties.Value.LastModified.DateTime,
                    ContentLength = properties.Value.ContentLength.ToString(),
                    FileType = GetFileType(latestBlob.Name)
                };

                result[folderPrefix.TrimEnd('/')] = model;
            }
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, LogMessage, $"{containerName} files");
        }

        return result;
    }

    public async Task<PublicRegisterFileModel> GetLatestFileAsync(string containerName, string blobName)
    {
        try
        {
            var fileModel = new PublicRegisterFileModel();
            var containerClient = GetContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var download = await blobClient.DownloadContentAsync();

            fileModel.FileContent = download.Value.Content.ToStream();
            fileModel.FileName = GetFileName(blobName);

            return fileModel;
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, LogMessage, $"{containerName} files");
            throw new PublicRegisterServiceException(string.Format(ErrorMessage, HomeNation.All), ex);
        }
    }

    [ExcludeFromCodeCoverage]
    public async Task<EnforcementActionFileViewModel> GetEnforcementActionFileByAgency(string agency)
    {
        var result = new EnforcementActionFileViewModel();
        var containerName = publicRegisterOptions.Value.EnforcementActionsBlobContainerName;

        try
        {
            var containerClient = GetContainerClient(containerName);

            if (containerClient is null) return result;

            // Used as a filter.
            var suffix = string.Format("_{0}", agency);

            // Get the file name value from the config file so the system knows what file name to look for.
            // Filename from blob storage without the suffix and extension must much what's stored in the config
            // otherwise the file won't be available for download. See user story (523627).

            var enforcementFileName = string.Format("{0}{1}", publicRegisterOptions.Value.EnforcementActionFileName, suffix);

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                if (Path.GetFileNameWithoutExtension(blobItem.Name) == enforcementFileName)
                {
                    result.FileName = blobItem.Name;
                    result.ContentFileLength = (int)blobItem.Properties.ContentLength;
                    result.DateCreated = blobItem.Properties.CreatedOn.Value.DateTime;
                    result.FileContents = await blobReader.DownloadBlobToStreamAsync(containerName, blobItem.Name, false);

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

    private static string GetFileName(string blobName)
    {
        var parts = blobName.Split('/');
        return parts.Length > 1 ? parts[1] : blobName;
    }

    private static async Task<BlobItem?> GetLatestBlobAsync(BlobContainerClient containerClient, string prefix)
    {
        BlobItem? latestBlob = null;

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var contentLength = blobItem.Properties?.ContentLength ?? 0;
            var lastModified = blobItem.Properties?.LastModified;

            if (contentLength > 0 && (latestBlob == null || 
                (lastModified != null && lastModified > latestBlob.Properties?.LastModified)))
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

    [ExcludeFromCodeCoverage]

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