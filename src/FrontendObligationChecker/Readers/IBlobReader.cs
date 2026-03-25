namespace FrontendObligationChecker.Readers;

using FrontendObligationChecker.Models.BlobReader;

public interface IBlobReader
{
    Task<Stream> DownloadBlobToStreamAsync(string fileName, bool prependBOM = false);

    Task<Stream> DownloadBlobToStreamAsync(string containerName, string fileName, bool prependBOM = false);

    Task<long> GetFileSizeInBytesAsync(string fileName);

    Task<IEnumerable<BlobModel>> GetBlobsAsync(string prefix);

    Task<IEnumerable<BlobModel>> GetBlobsAsync(string containerName, string? prefix);

    Task<IEnumerable<string>> GetDirectories();

    Task<IEnumerable<string>> GetDirectories(string containerName);

    Task<Stream> DownloadBlobContentAsync(string containerName, string blobName);
}