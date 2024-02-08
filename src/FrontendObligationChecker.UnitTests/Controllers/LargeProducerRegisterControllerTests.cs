namespace FrontendObligationChecker.UnitTests.Controllers;

using Constants;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;

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

    [TestInitialize]
    public void TestInitialize()
    {
        _largeProducerRegisterService = new Mock<ILargeProducerRegisterService>();
        _logger = new Mock<ILogger<LargeProducerRegisterController>>();
        _featureManager = new Mock<IFeatureManager>();

        _featureManager.Setup(x => x.IsEnabledAsync(It.IsRegex(NationLinkFeatureFlagRegex))).ReturnsAsync(true);

        _systemUnderTest = new LargeProducerRegisterController(_largeProducerRegisterService.Object, _logger.Object,
            _featureManager.Object);
    }

    [TestMethod]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalled()
    {
        // Arrange

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;

        // Assert
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

        // Act
        var result = await _systemUnderTest.FileNotDownloaded(nationCode) as NotFoundResult;

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _logger.VerifyLog(logger => logger.LogError(new ArgumentException(DisabledArgumentExceptionLog), InvalidArgumentExceptionLog, nationCode), Times.Once);
    }
}