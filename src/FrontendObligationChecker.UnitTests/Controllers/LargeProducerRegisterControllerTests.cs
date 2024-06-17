namespace FrontendObligationChecker.UnitTests.Controllers;

using Constants;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Services.Caching;
using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
public class LargeProducerRegisterControllerTests
{
    private const string FileNotDownloadedExceptionLog = "Failed to receive file for nation code {NationCode}";

    private Mock<ILargeProducerRegisterService> _largeProducerRegisterService;
    private Mock<ILogger<LargeProducerRegisterController>> _logger;
    private Mock<HttpContext>? _httpContextMock;
    private Mock<ISession> _sessionMock;

    private LargeProducerRegisterController _systemUnderTest;
    private Mock<ICacheService> _cacheServiceMock;

    [TestInitialize]
    public void TestInitialize()
    {
        _largeProducerRegisterService = new Mock<ILargeProducerRegisterService>();
        _logger = new Mock<ILogger<LargeProducerRegisterController>>();
        _cacheServiceMock = new Mock<ICacheService>();
        _sessionMock = new Mock<ISession>();
        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);
        _systemUnderTest = new LargeProducerRegisterController(_largeProducerRegisterService.Object, _logger.Object, _cacheServiceMock.Object);
        _systemUnderTest.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    [TestMethod]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalled()
    {
        // Arrange
        Dictionary<string, string> expectedCacheValue = new();
        _cacheServiceMock.Setup(c => c.GetReportFileSizeCache(It.IsAny<string>(), out expectedCacheValue))
            .Returns(false);
        _largeProducerRegisterService.Setup(x => x.GetAllReportFileSizesAsync(It.IsAny<string>()))
            .ReturnsAsync(GetFileSizeMappingDictionary());

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        LargeProducerRegisterViewModel viewModel = (LargeProducerRegisterViewModel)result.ViewData.Model;
        viewModel.HomeNationFileSizeMapping.Should().BeEquivalentTo(GetFileSizeMappingDictionary());
        result.ViewName.Should().Be("LargeProducerRegister");
        _largeProducerRegisterService.Verify(x => x.GetAllReportFileSizesAsync(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalledAndCacheExists()
    {
        // Arrange
        _largeProducerRegisterService.Setup(x => x.GetAllReportFileSizesAsync(It.IsAny<string>()))
            .ReturnsAsync((Dictionary<string, string>)null);
        Dictionary<string, string> expectedCacheValue = GetFileSizeMappingDictionary();
        _cacheServiceMock.Setup(c => c.GetReportFileSizeCache(It.IsAny<string>(), out expectedCacheValue))
            .Returns(true);

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

        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>(), It.IsAny<string>()))
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
        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new LargeProducerRegisterServiceException());

        // Act
        var result = await _systemUnderTest.File(nationCode) as RedirectToActionResult;

        // Assert
        result.ActionName.Should().Be("FileNotDownloaded");
        _logger.VerifyLog(logger => logger.LogError(FileNotDownloadedExceptionLog, nationCode), Times.Once);
    }

    [TestMethod]
    [DataRow(HomeNation.England)]
    [DataRow(HomeNation.NorthernIreland)]
    [DataRow(HomeNation.Scotland)]
    [DataRow(HomeNation.Wales)]
    [DataRow(HomeNation.All)]
    public async Task GetFileNotDownloaded_ReturnsLargeProducerErrorView_WhenCalledWithValidHomeNation(string nationCode)
    {
        // Act
        var result = await _systemUnderTest.FileNotDownloaded(nationCode) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("LargeProducerError");
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