namespace OutsideInTests.Infrastructure;

using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Readers;

/// <summary>
/// Controllable stub replacing real Azure Blob Storage access at the IBlobReader level.
/// Tests configure responses via the public properties before making requests.
/// This sits below LargeProducerRegisterService, so that service's real logic
/// (directory listing, prefix matching, caching, file size formatting) is exercised.
/// </summary>
public class StubBlobReader : IBlobReader
{
    /// <summary>
    /// Blobs keyed by name. GetBlobsAsync filters by prefix, GetDirectories
    /// derives folder names from the blob names.
    /// </summary>
    public List<BlobModel> Blobs { get; set; } = [];

    /// <summary>
    /// File sizes keyed by file name, returned by GetFileSizeInBytesAsync.
    /// </summary>
    public Dictionary<string, long> FileSizes { get; set; } = new();

    /// <summary>
    /// Streams keyed by file name, returned by DownloadBlobToStreamAsync.
    /// </summary>
    public Dictionary<string, byte[]> FileContents { get; set; } = new();

    public Task<IEnumerable<BlobModel>> GetBlobsAsync(string prefix)
    {
        var filtered = Blobs.Where(b => b.Name != null && b.Name.StartsWith(prefix));
        return Task.FromResult<IEnumerable<BlobModel>>(filtered.ToList());
    }

    public Task<IEnumerable<string>> GetDirectories()
    {
        var dirs = Blobs
            .Where(b => b.Name != null && b.Name.Contains('/'))
            .Select(b => b.Name.Split('/')[0] + "/")
            .Distinct();
        return Task.FromResult<IEnumerable<string>>(dirs.ToList());
    }

    public Task<long> GetFileSizeInBytesAsync(string fileName)
    {
        return Task.FromResult(FileSizes.GetValueOrDefault(fileName, 0L));
    }

    public Task<Stream> DownloadBlobToStreamAsync(string fileName, bool prependBOM = false)
    {
        var content = FileContents.GetValueOrDefault(fileName, []);
        Stream stream = new MemoryStream(content);
        return Task.FromResult(stream);
    }

    public Task<Stream> DownloadBlobToStreamAsync(string containerName, string fileName, bool prependBOM = false)
    {
        return DownloadBlobToStreamAsync(fileName, prependBOM);
    }

    public void Reset()
    {
        Blobs = [];
        FileSizes = new();
        FileContents = new();
    }
}
