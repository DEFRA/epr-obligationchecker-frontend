namespace FrontendObligationChecker.UnitTests.Controllers;

using Constants;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

using Moq;
using ViewModels;

[TestClass]
public class LargeProducerRegisterControllerTests
{
    private const string NationLinkFeatureFlagRegex = "([A-Za-z])*Nations?DownloadLink";
    private const string FileNotDownloadedExceptionLog = "Failed to receive file for nation code {NationCode}";
    private const string InvalidArgumentExceptionLog = "Nation code {NationCode} does not exist";
    private const string DisabledArgumentExceptionLog = "Nation code {NationCode} is disabled by feature flag";

    private Mock<ILargeProducerRegisterService> _largeProducerRegisterService;
    private Mock<ILogger<LargeProducerRegisterController>> _logger;
    private Mock<IFeatureManager> _featureManager;

    private LargeProducerRegisterController _systemUnderTest;
    private IOptions<CachingOptions> _cachingOptions;
    private IMemoryCache _memoryCache;

    [TestInitialize]
    public void TestInitialize()
    {
        _largeProducerRegisterService = new Mock<ILargeProducerRegisterService>();
        _logger = new Mock<ILogger<LargeProducerRegisterController>>();
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
    }

    private LargeProducerRegisterController GetSystemUnderTest()
    {
        return new LargeProducerRegisterController(_largeProducerRegisterService.Object, _logger.Object, _featureManager.Object, _memoryCache, _cachingOptions);
    }

    [TestMethod]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalled()
    {
        // Arrange
        _largeProducerRegisterService.Setup(x => x.GetAllReportFileSizesAsync())
            .ReturnsAsync(GetFileSizeMappingDictionary());
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        LargeProducerRegisterViewModel viewModel = (LargeProducerRegisterViewModel)result.ViewData.Model;
        viewModel.HomeNationFileSizeMapping.Should().BeEquivalentTo(GetFileSizeMappingDictionary());
        result.ViewName.Should().Be("LargeProducerRegister");
        _largeProducerRegisterService.Verify(x => x.GetAllReportFileSizesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalledAndCacheExists()
    {
        // Arrange
        _largeProducerRegisterService.Setup(x => x.GetAllReportFileSizesAsync())
            .ReturnsAsync(GetFileSizeMappingDictionary());
        _memoryCache.Set("FileSizeMetadataCacheKey", GetFileSizeMappingDictionary());
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;

        // Assert
        LargeProducerRegisterViewModel viewModel = (LargeProducerRegisterViewModel)result.ViewData.Model;
        viewModel.HomeNationFileSizeMapping.Should().BeEquivalentTo(GetFileSizeMappingDictionary());
        result.Should().NotBeNull();
        result.ViewName.Should().Be("LargeProducerRegister");
    }

    [TestMethod]
    [DataRow(HomeNation.England)]
    [DataRow(HomeNation.NorthernIreland)]
    [DataRow(HomeNation.Scotland)]
    [DataRow(HomeNation.Wales)]
    [DataRow(HomeNation.All)]
    public async Task GetFile_ValidNationCodeAndLargeProducerRegisterServiceReturnsFile_ReturnFile(string nationCode)
    {
        // Arrange
        const string fileName = "example error report.csv";
        var memoryStream = new MemoryStream();

        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>()))
            .ReturnsAsync((memoryStream, fileName));
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.File(nationCode) as FileStreamResult;

        // Assert
        result.FileDownloadName.Should().Be(fileName);
        result.FileStream.Should().BeSameAs(memoryStream);
        result.ContentType.Should().Be("text/csv");
    }

    [TestMethod]
    [DataRow(HomeNation.England)]
    [DataRow(HomeNation.NorthernIreland)]
    [DataRow(HomeNation.Scotland)]
    [DataRow(HomeNation.Wales)]
    [DataRow(HomeNation.All)]
    public async Task GetFile_LargeProducerRegisterServiceThrowsException_RedirectToLargeProducerErrorPage(string nationCode)
    {
        // Arrange
        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>()))
            .ThrowsAsync(new LargeProducerRegisterServiceException());
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.File(nationCode) as RedirectToActionResult;

        // Assert
        result.ActionName.Should().Be("FileNotDownloaded");
        _logger.VerifyLog(logger => logger.LogError(FileNotDownloadedExceptionLog, nationCode), Times.Once);
    }

    [TestMethod]
    [DataRow(HomeNation.England, FeatureFlags.EnglandNationDownloadLink)]
    [DataRow(HomeNation.NorthernIreland, FeatureFlags.NorthernIrelandNationDownloadLink)]
    [DataRow(HomeNation.Scotland, FeatureFlags.ScotlandNationDownloadLink)]
    [DataRow(HomeNation.Wales, FeatureFlags.WalesNationDownloadLink)]
    [DataRow(HomeNation.All, FeatureFlags.AllNationsDownloadLink)]
    public async Task GetFile_NationCodeDisabledByFeatureFlag_Returns404(string nationCode, string featureFlag)
    {
        // Arrange
        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>()))
            .ThrowsAsync(new LargeProducerRegisterServiceException());
        _featureManager.Setup(x => x.IsEnabledAsync(It.IsRegex(featureFlag))).ReturnsAsync(false);
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.File(nationCode) as NotFoundResult;

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _logger.VerifyLog(logger => logger.LogError(new ArgumentException(DisabledArgumentExceptionLog), InvalidArgumentExceptionLog, nationCode), Times.Once);
    }

    [TestMethod]
    public async Task GetFile_InvalidNationCode_Returns404()
    {
        // Arrange
        const string nationCode = "XX";

        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>()))
            .ThrowsAsync(new LargeProducerRegisterServiceException());
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.File(nationCode) as NotFoundResult;

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _logger.VerifyLog(logger => logger.LogError(InvalidArgumentExceptionLog, nationCode), Times.Once);
    }

    [TestMethod]
    [DataRow(HomeNation.England)]
    [DataRow(HomeNation.NorthernIreland)]
    [DataRow(HomeNation.Scotland)]
    [DataRow(HomeNation.Wales)]
    [DataRow(HomeNation.All)]
    public async Task GetFileNotDownloaded_ReturnsLargeProducerErrorView_WhenCalledWithValidHomeNation(string nationCode)
    {
        // Arrange
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.FileNotDownloaded(nationCode) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("LargeProducerError");
    }

    [TestMethod]
    public async Task GetFileNotDownloaded_Returns404_WhenCalledWithInvalidHomeNation()
    {
        // Arrange
        const string nationCode = "XX";
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.FileNotDownloaded(nationCode) as NotFoundResult;

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _logger.VerifyLog(logger => logger.LogError(InvalidArgumentExceptionLog, nationCode), Times.Once);
    }

    [TestMethod]
    [DataRow(HomeNation.England, FeatureFlags.EnglandNationDownloadLink)]
    [DataRow(HomeNation.NorthernIreland, FeatureFlags.NorthernIrelandNationDownloadLink)]
    [DataRow(HomeNation.Scotland, FeatureFlags.ScotlandNationDownloadLink)]
    [DataRow(HomeNation.Wales, FeatureFlags.WalesNationDownloadLink)]
    [DataRow(HomeNation.All, FeatureFlags.AllNationsDownloadLink)]
    public async Task GetFileNotDownloaded_Returns404_WhenCalledWithCorrespondingFeatureFlagDisabledHomeNation(string nationCode, string featureFlag)
    {
        // Arrange
        _featureManager.Setup(x => x.IsEnabledAsync(It.IsRegex(featureFlag))).ReturnsAsync(false);
        _systemUnderTest = GetSystemUnderTest();

        // Act
        var result = await _systemUnderTest.FileNotDownloaded(nationCode) as NotFoundResult;

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _logger.VerifyLog(logger => logger.LogError(new ArgumentException(DisabledArgumentExceptionLog), InvalidArgumentExceptionLog, nationCode), Times.Once);
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