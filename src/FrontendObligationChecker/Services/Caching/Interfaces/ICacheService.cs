namespace FrontendObligationChecker.Services.Caching;

using FrontendObligationChecker.Models.BlobReader;

public interface ICacheService
{
    bool GetReportFileSizeCache(string culture, out Dictionary<string, string> reportFileSizeMapping);

    void SetReportFileSizeCache(string culture, Dictionary<string, string> reportFileSizeMapping);

    bool GetReportDirectoriesCache(out IEnumerable<string> reportDirectories);

    void SetReportDirectoriesCache(IEnumerable<string> reportDirectories);

    bool GetBlobModelCache(string prefix, out BlobModel blobModel);

    void SetBlobModelCache(string prefix, BlobModel blobModel);
}