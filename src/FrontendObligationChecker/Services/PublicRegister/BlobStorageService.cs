namespace FrontendObligationChecker.Services.PublicRegister;

using System.Diagnostics.CodeAnalysis;
using FrontendObligationChecker.Constants;
using FrontendObligationChecker.Exceptions;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Readers;
using FrontendObligationChecker.ViewModels.PublicRegister;
using Microsoft.Extensions.Options;

public class BlobStorageService(
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
            if (string.IsNullOrWhiteSpace(containerName)) return result;

            var directories = await blobReader.GetDirectories(containerName);
            var latestFolderPrefix = directories
                .OrderByDescending(p => p)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(latestFolderPrefix)) return result;

            var latestBlob = await GetLatestBlobAsync(containerName, latestFolderPrefix);
            if (latestBlob is null) return result;

            result.Name = latestBlob.Name;
            result.LastModified = latestBlob.LastModified;
            result.ContentLength = latestBlob.ContentLength?.ToString();
            result.FileType = GetFileType(latestBlob.Name);

            return result;
        }
        catch (BlobReaderException ex)
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
            if (string.IsNullOrWhiteSpace(containerName)) return result;

            foreach (var folderPrefix in folderPrefixes)
            {
                if (string.IsNullOrWhiteSpace(folderPrefix)) continue;

                var latestBlob = await GetLatestBlobAsync(containerName, folderPrefix);
                if (latestBlob is null) continue;

                var model = new PublicRegisterBlobModel
                {
                    PublishedDate = publicRegisterOptions.Value.PublishedDate,
                    Name = latestBlob.Name,
                    LastModified = latestBlob.LastModified,
                    ContentLength = latestBlob.ContentLength?.ToString(),
                    FileType = GetFileType(latestBlob.Name)
                };

                result[folderPrefix.TrimEnd('/')] = model;
            }
        }
        catch (BlobReaderException ex)
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
            fileModel.FileContent = await blobReader.DownloadBlobContentAsync(containerName, blobName);
            fileModel.FileName = GetFileName(blobName);

            return fileModel;
        }
        catch (BlobReaderException ex)
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
            if (string.IsNullOrWhiteSpace(containerName)) return result;

            var suffix = string.Format("_{0}", agency);
            var enforcementFileName = string.Format("{0}{1}", publicRegisterOptions.Value.EnforcementActionFileName, suffix);

            var blobs = await blobReader.GetBlobsAsync(containerName, null);

            foreach (var blob in blobs)
            {
                if (Path.GetFileNameWithoutExtension(blob.Name) == enforcementFileName)
                {
                    result.FileName = blob.Name;
                    result.ContentFileLength = (int)(blob.ContentLength ?? 0);
                    result.DateCreated = blob.CreatedOn ?? DateTime.MinValue;
                    result.FileContents = await blobReader.DownloadBlobToStreamAsync(containerName, blob.Name, false);

                    break;
                }
            }
        }
        catch (BlobReaderException ex)
        {
            logger.LogError(ex, LogMessage, $"{containerName} files");
        }

        return result;
    }

    [ExcludeFromCodeCoverage]
    public async Task<IEnumerable<EnforcementActionFileViewModel>> GetEnforcementActionFiles()
    {
        var results = new List<EnforcementActionFileViewModel>();
        var containerName = publicRegisterOptions.Value.EnforcementActionsBlobContainerName;

        try
        {
            if (string.IsNullOrWhiteSpace(containerName)) return results;

            var blobs = await blobReader.GetBlobsAsync(containerName, null);

            foreach (var blob in blobs)
            {
                results.Add(new EnforcementActionFileViewModel
                {
                    FileName = blob.Name,
                    DateCreated = DateTime.Now,
                    ContentFileLength = (int)(blob.ContentLength ?? 0)
                });
            }
        }
        catch (BlobReaderException ex)
        {
            logger.LogError(ex, LogMessage, $"{containerName} files");
            throw;
        }

        return results;
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

    private async Task<BlobModel?> GetLatestBlobAsync(string containerName, string prefix)
    {
        var blobs = await blobReader.GetBlobsAsync(containerName, prefix);

        return blobs
            .Where(b => (b.ContentLength ?? 0) > 0 && b.LastModified != null)
            .MaxBy(b => b.LastModified);
    }
}
