namespace FrontendObligationChecker.Services.Caching;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Models.Config;

public class CacheService : ICacheService
{
    private const string CacheKey = "FileSizeMetadataCacheKey";
    private const string RetrievedFileSizeCache = "File size has been retrieved by cache.";
    private const string FileSizeCacheHasBeenSet = "File size cache has been set.";
    private readonly IMemoryCache _cache;
    private readonly CachingOptions _cachingOptions;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, IOptions<CachingOptions> cachingOptions, ILogger<CacheService> logger)
    {
        _cache = cache;
        _cachingOptions = cachingOptions.Value;
        _logger = logger;
    }

    public bool GetReportFileSizeCache(string culture, out Dictionary<string, string> fileSizeCacheMapping)
    {
        _cache.TryGetValue(GetCacheKey(culture), out fileSizeCacheMapping);
        if (fileSizeCacheMapping is null)
        {
            return false;
        }

        _logger.LogInformation(RetrievedFileSizeCache);
        return true;
    }

    public void SetReportFileSizeCache(string culture, Dictionary<string, string> reportFileSizeMapping)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(_cachingOptions.ProducerReportFileSizeDays));
        _cache.Set(GetCacheKey(culture), reportFileSizeMapping, cacheEntryOptions);
        _logger.LogInformation(FileSizeCacheHasBeenSet);
    }

    private static string GetCacheKey(string culture)
    {
        return $"{culture}-{CacheKey}";
    }
}