namespace OutsideInTests.Infrastructure;

using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Readers;

/// <summary>
/// Controllable stub replacing real Azure Blob Storage access at the IBlobReader level.
/// Tests configure responses via the public properties before making requests.
/// Both LargeProducerRegisterService and BlobStorageService use IBlobReader,
/// so their real logic (directory listing, prefix matching, caching, file size
/// formatting, latest-blob selection) is exercised through this stub.
/// </summary>
public class StubBlobReader : IBlobReader
{
    /// <summary>
    /// Blobs for the default container (used by LargeProducerRegisterService).
    /// GetBlobsAsync(prefix) filters by prefix, GetDirectories() derives folder names.
    /// </summary>
    public List<BlobModel> Blobs { get; set; } = [];

    /// <summary>
    /// Blobs keyed by container name (used by BlobStorageService).
    /// </summary>
    public Dictionary<string, List<BlobModel>> ContainerBlobs { get; set; } = new();

    /// <summary>
    /// File sizes keyed by file name, returned by GetFileSizeInBytesAsync.
    /// </summary>
    public Dictionary<string, long> FileSizes { get; set; } = new();

    /// <summary>
    /// File contents keyed by file name, returned by download methods.
    /// </summary>
    public Dictionary<string, byte[]> FileContents { get; set; } = new();

    public Task<IEnumerable<BlobModel>> GetBlobsAsync(string prefix)
    {
        var filtered = Blobs.Where(b => b.Name != null && b.Name.StartsWith(prefix));
        return Task.FromResult<IEnumerable<BlobModel>>(filtered.ToList());
    }

    public Task<IEnumerable<BlobModel>> GetBlobsAsync(string containerName, string? prefix)
    {
        if (!ContainerBlobs.TryGetValue(containerName, out var blobs))
        {
            return Task.FromResult<IEnumerable<BlobModel>>([]);
        }

        var filtered = string.IsNullOrEmpty(prefix)
            ? blobs
            : blobs.Where(b => b.Name != null && b.Name.StartsWith(prefix)).ToList();
        return Task.FromResult<IEnumerable<BlobModel>>(filtered);
    }

    public Task<IEnumerable<string>> GetDirectories()
    {
        var dirs = Blobs
            .Where(b => b.Name != null && b.Name.Contains('/'))
            .Select(b => b.Name.Split('/')[0] + "/")
            .Distinct();
        return Task.FromResult<IEnumerable<string>>(dirs.ToList());
    }

    public Task<IEnumerable<string>> GetDirectories(string containerName)
    {
        if (!ContainerBlobs.TryGetValue(containerName, out var blobs))
        {
            return Task.FromResult<IEnumerable<string>>([]);
        }

        var dirs = blobs
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

    public Task<Stream> DownloadBlobContentAsync(string containerName, string blobName)
    {
        var content = FileContents.GetValueOrDefault(blobName, []);
        Stream stream = new MemoryStream(content);
        return Task.FromResult(stream);
    }

    public void Reset()
    {
        Blobs = [];
        ContainerBlobs = new();
        FileSizes = new();
        FileContents = new();
    }
}
