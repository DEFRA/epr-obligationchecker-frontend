namespace FrontendObligationChecker.UnitTests.Controllers;

using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Constants;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using FrontendObligationChecker.ViewModels;
using FrontendObligationChecker.ViewModels.LargeProducer;
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

    [TestInitialize]
    public void TestInitialize()
    {
        _largeProducerRegisterService = new Mock<ILargeProducerRegisterService>();
        _logger = new Mock<ILogger<LargeProducerRegisterController>>();
        _sessionMock = new Mock<ISession>();
        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);
        _systemUnderTest = new LargeProducerRegisterController(_largeProducerRegisterService.Object, _logger.Object);
        _systemUnderTest.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    [TestMethod]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalled()
    {
        // Arrange
        var expected = new List<LargeProducerFileInfoViewModel>
        {
            new LargeProducerFileInfoViewModel
            {
                DisplayFileSize = "2MB",
                DateCreated = new DateTime(1970, 1, 1)
            }
        };

        _largeProducerRegisterService.Setup(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        LargeProducerRegisterViewModel viewModelResult = (LargeProducerRegisterViewModel)result.ViewData.Model;
        viewModelResult.LatestAllNationsFiles.Should().BeEquivalentTo(expected);
        result.ViewName.Should().Be("LargeProducerRegister");
        _largeProducerRegisterService.Verify(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task GetFile_WhenReportingYearIsNull_RedirectBackToLargeProducersPage()
    {
        // Act
        var result = await _systemUnderTest.File(null) as RedirectToActionResult;

        // Assert
        result.ActionName.Should().Be("Get");
    }

    [TestMethod]
    public async Task GetFile_LargeProducerRegisterServiceReturnsFile_ReturnFile()
    {
        // Arrange
        const int ReportingYear = 1970;

        var viewModel = new LargeProducerFileViewModel
        {
            FileName = "example error report.csv",
            FileContents = new MemoryStream()
        };

        _largeProducerRegisterService.Setup(x => x.GetLatestAllNationsFileAsync(ReportingYear, It.IsAny<string>())).ReturnsAsync(viewModel);

        // Act
        var result = await _systemUnderTest.File(ReportingYear) as FileStreamResult;

        // Assert
        result.FileDownloadName.Should().Be(viewModel.FileName);
        result.FileStream.Should().BeSameAs(viewModel.FileContents);
        result.ContentType.Should().Be("text/csv");
    }

    [TestMethod]
    public async Task GetFile_LargeProducerRegisterServiceThrowsException_RedirectToLargeProducerErrorPage()
    {
        // Arrange
        const int ReportingYear = 1970;

        _largeProducerRegisterService.Setup(x => x.GetLatestAllNationsFileAsync(ReportingYear, It.IsAny<string>()))
            .ThrowsAsync(new LargeProducerRegisterServiceException());

        // Act
        var result = await _systemUnderTest.File(ReportingYear) as RedirectToActionResult;

        // Assert
        result.ActionName.Should().Be("FileNotDownloaded");
        _logger.VerifyLog(logger => logger.LogError(FileNotDownloadedExceptionLog, HomeNation.All), Times.Once);
    }

    [TestMethod]
    public async Task GetFile_FileNotFound_RedirectToLargeProducerErrorPage()
    {
        // Arrange
        const int ReportingYear = 1970;

        _largeProducerRegisterService.Setup(x => x.GetLatestAllNationsFileAsync(ReportingYear, It.IsAny<string>()))
            .ReturnsAsync((LargeProducerFileViewModel)null);

        // Act
        var result = await _systemUnderTest.File(ReportingYear) as RedirectToActionResult;

        // Assert
        result.ActionName.Should().Be("FileNotDownloaded");
    }

    [TestMethod]
    public async Task GetFileNotDownloaded_ReturnsLargeProducerErrorView_WhenCalledWithValidHomeNation()
    {
        // Act
        var result = await _systemUnderTest.FileNotDownloaded() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("LargeProducerError");
    }
}