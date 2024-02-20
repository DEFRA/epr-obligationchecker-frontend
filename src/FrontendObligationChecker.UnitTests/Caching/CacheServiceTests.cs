namespace FrontendObligationChecker.UnitTests.Caching;

using Constants;
using FluentAssertions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Services.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;

[TestClass]
public class CacheServiceTests
{
    private const string NationLinkFeatureFlagRegex = "([A-Za-z])*Nations?DownloadLink";
    private CacheService _systemUnderTest;
    private Mock<ILogger<CacheService>> _logger;
    private Mock<IFeatureManager> _featureManager;
    private IOptions<CachingOptions> _cachingOptions;
    private IMemoryCache _memoryCache;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<ILogger<CacheService>>();

        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        _memoryCache = serviceProvider.GetService<IMemoryCache>();

        _featureManager = new Mock<IFeatureManager>();
        _featureManager.Setup(x => x.IsEnabledAsync(It.IsRegex(NationLinkFeatureFlagRegex))).ReturnsAsync(true);

        _cachingOptions = Options.Create(
            new CachingOptions()
            {
                ProducerReportFileSizeDays = 1
            });

        _systemUnderTest = new CacheService(_memoryCache, _cachingOptions, _logger.Object);
    }

    [TestMethod]
    public async Task GetCacheReportFileSizeCache_ReturnsCache_WhenCalledAndEnglishCacheExists()
    {
        // Arrange
        _memoryCache.Set("en-FileSizeMetadataCacheKey", GetFileSizeMappingDictionary());

        // Act
        var result = _systemUnderTest.GetReportFileSizeCache("en", out Dictionary<string, string> fileSizeCacheMapping);

        // Assert
        result.Should().BeTrue();
        fileSizeCacheMapping.Should().BeEquivalentTo(GetFileSizeMappingDictionary());
    }

    [TestMethod]
    public async Task GetCacheReportFileSizeCache_ReturnsCache_WhenCalledAndWelshCacheExists()
    {
        // Arrange
        _memoryCache.Set("cy-FileSizeMetadataCacheKey", GetFileSizeMappingDictionary());

        // Act
        var result = _systemUnderTest.GetReportFileSizeCache("cy", out Dictionary<string, string> fileSizeCacheMapping);

        // Assert
        result.Should().BeTrue();
        fileSizeCacheMapping.Should().BeEquivalentTo(GetFileSizeMappingDictionary());
    }

    [TestMethod]
    public async Task GetCacheReportFileSizeCache_ReturnsFalse_WhenCalledAndCacheDoesNotExists()
    {
        // Act
        var result = _systemUnderTest.GetReportFileSizeCache("en", out Dictionary<string, string> fileSizeCacheMapping);

        // Assert
        result.Should().BeFalse();
        fileSizeCacheMapping.Should().BeNull();
    }

    [TestMethod]
    public async Task SetCacheReportFileSizeCache_UpdatesCache_WhenCalled()
    {
        // Act
        _systemUnderTest.SetReportFileSizeCache("en", GetFileSizeMappingDictionary());

        // Assert
        _memoryCache.TryGetValue("en-FileSizeMetadataCacheKey", out Dictionary<string, string> fileSizeMapping);
        fileSizeMapping.Should().BeEquivalentTo(GetFileSizeMappingDictionary());
    }

    private static Dictionary<string, string> GetFileSizeMappingDictionary()
    {
        return new Dictionary<string, string>()
        {
            {
                HomeNation.England, "10MB"
            },
            {
                HomeNation.Scotland, "10KB"
            },
            {
                HomeNation.Wales, "10MB"
            },
            {
                HomeNation.NorthernIreland, "10MB"
            },
            {
                HomeNation.All, "10MB"
            }
        };
    }
}