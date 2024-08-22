namespace FrontendObligationChecker.Services.Caching;

using System.Collections.Generic;
using FrontendObligationChecker.Models.BlobReader;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Models.Config;

public class CacheService : ICacheService
{
    private const string CacheKey = "FileSizeMetadataCacheKey";
    private const string ReportDirectoriesCacheKey = "ReportDirectoriesCacheKey";
    private const string BlobModelCacheKey = "BlobModelCacheKey";

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

    public bool GetReportDirectoriesCache(out IEnumerable<string> reportDirectories)
    {
        _cache.TryGetValue(ReportDirectoriesCacheKey, out reportDirectories);

        if (reportDirectories is null)
        {
            return false;
        }

        _logger.LogInformation("Report directories have been retrieved by cache.");

        return true;
    }

    public void SetReportDirectoriesCache(IEnumerable<string> reportDirectories)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(GetExpiryDateForLargeProducersData());

        _cache.Set(ReportDirectoriesCacheKey, reportDirectories, cacheEntryOptions);

        _logger.LogInformation("Report directories cache has been set.");
    }

    public bool GetBlobModelCache(string prefix, out BlobModel blobModel)
    {
        _cache.TryGetValue(GetBlobModelCacheKey(prefix), out blobModel);

        if (blobModel is null)
        {
            return false;
        }

        _logger.LogInformation("Blob model has been retrieved by cache.");

        return true;
    }

    public void SetBlobModelCache(string prefix, BlobModel blobModel)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(GetExpiryDateForLargeProducersData());

        _cache.Set(GetBlobModelCacheKey(prefix), blobModel, cacheEntryOptions);

        _logger.LogInformation("Blob model cache has been set.");
    }

    private static string GetCacheKey(string culture)
    {
        return $"{culture}-{CacheKey}";
    }

    private static string GetBlobModelCacheKey(string prefix)
    {
        return $"{BlobModelCacheKey}-{prefix}";
    }

    private DateTime GetExpiryDateForLargeProducersData()
    {
        var expiryDate = new DateTime(
           DateTime.Now.Year,
           DateTime.Now.Month,
           DateTime.Now.Day,
           _cachingOptions.LargeProducersFileGeneratedHour,
           0,
           0,
           DateTimeKind.Local);

        expiryDate = expiryDate.AddMinutes(_cachingOptions.LargeProducersFileGenerationWaitMinutes);

        var currentDayMinutes = (DateTime.Now.Hour * 60) + DateTime.Now.Minute;
        var newFileGeneratedAtDayMinutes = (_cachingOptions.LargeProducersFileGeneratedHour * 60) + _cachingOptions.LargeProducersFileGenerationWaitMinutes;

        if (currentDayMinutes >= newFileGeneratedAtDayMinutes)
        {
            expiryDate = expiryDate.AddDays(1);
        }

        return expiryDate;
    }
}