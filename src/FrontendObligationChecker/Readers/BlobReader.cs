namespace FrontendObligationChecker.Readers;

using System.Collections.Generic;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Exceptions;
using FrontendObligationChecker.Helpers;
using FrontendObligationChecker.Models.BlobReader;

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

    public async Task<Stream> DownloadBlobToStreamAsync(string fileName, bool prependBOM = false)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            memoryStream.Position = 0;

            if (prependBOM)
            {
                BomHelper.PrependBOMBytes(memoryStream);
            }

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

    public async Task<IEnumerable<BlobModel>> GetBlobsAsync(string prefix)
    {
        try
        {
            var list = new List<BlobModel>();

            var resultSegment = _blobContainerClient.GetBlobsAsync(prefix: prefix).AsPages(default);

            await foreach (Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    list.Add(new BlobModel
                    {
                        Name = blobItem.Name,
                        ContentLength = blobItem.Properties.ContentLength,
                        CreatedOn = blobItem.Properties.CreatedOn?.LocalDateTime,
                    });
                }
            }

            return list;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, LogMessage, prefix);
            throw new BlobReaderException(string.Format(ErrorMessage, prefix), ex);
        }
    }

    public async Task<IEnumerable<string>> GetDirectories()
    {
        try
        {
            var directories = new List<string>();

            var resultSegment = _blobContainerClient.GetBlobsByHierarchyAsync(delimiter: "/").AsPages(default);

            await foreach (Page<BlobHierarchyItem> blobHierarchyItemPage in resultSegment)
            {
                foreach (BlobHierarchyItem blobHierarchyItem in blobHierarchyItemPage.Values)
                {
                    if (blobHierarchyItem.IsPrefix)
                    {
                        directories.Add(blobHierarchyItem.Prefix);
                    }
                }
            }

            return directories;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, LogMessage, "directories");
            throw new BlobReaderException(string.Format(ErrorMessage, "directories"), ex);
        }
    }
}