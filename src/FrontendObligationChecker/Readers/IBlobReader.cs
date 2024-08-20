namespace FrontendObligationChecker.Readers;

using FrontendObligationChecker.Models.BlobReader;

public interface IBlobReader
{
    Task<Stream> DownloadBlobToStreamAsync(string fileName);

    Task<long> GetFileSizeInBytesAsync(string fileName);

    Task<IEnumerable<BlobModel>> GetBlobsAsync(string prefix);

    Task<IEnumerable<string>> GetDirectories();
}