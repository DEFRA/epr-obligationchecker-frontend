namespace FrontendObligationChecker.UnitTests.Controllers;

using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Constants;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using FrontendObligationChecker.Services.PublicRegister;
using FrontendObligationChecker.ViewModels;
using FrontendObligationChecker.ViewModels.LargeProducer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class LargeProducerRegisterControllerTests
{
    private const string FileNotDownloadedExceptionLog = "Failed to receive file for nation code {NationCode}";
    private const string TestContainerName = "test-container";

    private Mock<ILargeProducerRegisterService> _largeProducerRegisterService;
    private Mock<IBlobStorageService> _blobStorageService;
    private Mock<ILogger<LargeProducerRegisterController>> _logger;
    private Mock<HttpContext>? _httpContextMock;
    private Mock<ISession> _sessionMock;
    private IOptions<PublicRegisterOptions> _publicRegisterOptions;

    private LargeProducerRegisterController _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _largeProducerRegisterService = new Mock<ILargeProducerRegisterService>();
        _blobStorageService = new Mock<IBlobStorageService>();
        _logger = new Mock<ILogger<LargeProducerRegisterController>>();
        _sessionMock = new Mock<ISession>();
        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);

        _publicRegisterOptions = Options.Create(new PublicRegisterOptions
        {
            PublicRegisterBlobContainerName = TestContainerName,
            CurrentYear = DateTime.UtcNow.Year.ToString()
        });

        _blobStorageService
            .Setup(x => x.GetLatestFilePropertiesAsync(It.IsAny<string>(), It.IsAny<List<string>>()))
            .ReturnsAsync(new Dictionary<string, PublicRegisterBlobModel>());

        _systemUnderTest = new LargeProducerRegisterController(
            _largeProducerRegisterService.Object,
            _blobStorageService.Object,
            _publicRegisterOptions,
            _logger.Object);

        _systemUnderTest.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    [TestMethod]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalled()
    {
        // Arrange
        var serviceFiles = new List<LargeProducerFileInfoViewModel>
        {
            new LargeProducerFileInfoViewModel { ReportingYear = 2024, DisplayFileSize = "2MB", DateCreated = new DateTime(2024, 1, 1) },
            new LargeProducerFileInfoViewModel { ReportingYear = 2023, DisplayFileSize = "1MB", DateCreated = new DateTime(2023, 1, 1) }
        };

        _largeProducerRegisterService
            .Setup(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(serviceFiles);

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("LargeProducerRegister");
        _largeProducerRegisterService.Verify(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public async Task Get_ListOfLargeProducers_ContainsOnlyYearsBefore2025()
    {
        // Arrange
        var serviceFiles = new List<LargeProducerFileInfoViewModel>
        {
            new LargeProducerFileInfoViewModel { ReportingYear = 2023, DisplayFileSize = "1MB", DateCreated = new DateTime(2023, 1, 1) },
            new LargeProducerFileInfoViewModel { ReportingYear = 2024, DisplayFileSize = "2MB", DateCreated = new DateTime(2024, 1, 1) },
            new LargeProducerFileInfoViewModel { ReportingYear = 2025, DisplayFileSize = "3MB", DateCreated = new DateTime(2025, 1, 1) }
        };

        _largeProducerRegisterService
            .Setup(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(serviceFiles);

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;
        var viewModel = result.ViewData.Model as LargeProducerRegisterViewModel;

        // Assert
        viewModel.ListOfLargeProducers.Should().OnlyContain(x => x.ReportingYear < 2025);
        viewModel.ListOfLargeProducers.Should().HaveCount(2);
    }

    [TestMethod]
    public async Task Get_ListOfLargeProducers_SetsCorrectDownloadUrl()
    {
        // Arrange
        var serviceFiles = new List<LargeProducerFileInfoViewModel>
        {
            new LargeProducerFileInfoViewModel { ReportingYear = 2024, DisplayFileSize = "2MB", DateCreated = new DateTime(2024, 1, 1) }
        };

        _largeProducerRegisterService
            .Setup(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(serviceFiles);

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;
        var viewModel = result.ViewData.Model as LargeProducerRegisterViewModel;

        // Assert
        viewModel.ListOfLargeProducers.Should().ContainSingle()
            .Which.DownloadUrl.Should().Be("/large-producers/report?reportingYear=2024");
    }

    [TestMethod]
    public async Task Get_RegisterOfProducers_SourcedFromBlobStorage()
    {
        // Arrange
        _largeProducerRegisterService
            .Setup(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<LargeProducerFileInfoViewModel>());

        var blobResult = new Dictionary<string, PublicRegisterBlobModel>
        {
            ["2025"] = new PublicRegisterBlobModel
            {
                Name = "2025/producers.csv",
                ContentLength = "1048576",
                LastModified = new DateTime(2025, 4, 1)
            }
        };

        _blobStorageService
            .Setup(x => x.GetLatestFilePropertiesAsync(TestContainerName, It.IsAny<List<string>>()))
            .ReturnsAsync(blobResult);

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;
        var viewModel = result.ViewData.Model as LargeProducerRegisterViewModel;

        // Assert
        viewModel.RegisterOfProducers.Should().ContainSingle()
            .Which.ReportingYear.Should().Be(2025);
    }

    [TestMethod]
    public async Task Get_RegisterOfProducers_SetsCorrectDownloadUrl()
    {
        // Arrange
        _largeProducerRegisterService
            .Setup(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<LargeProducerFileInfoViewModel>());

        var blobResult = new Dictionary<string, PublicRegisterBlobModel>
        {
            ["2025"] = new PublicRegisterBlobModel
            {
                Name = "2025/producers.csv",
                ContentLength = "1048576",
                LastModified = new DateTime(2025, 4, 1)
            }
        };

        _blobStorageService
            .Setup(x => x.GetLatestFilePropertiesAsync(TestContainerName, It.IsAny<List<string>>()))
            .ReturnsAsync(blobResult);

        // Act
        var result = await _systemUnderTest.Get() as ViewResult;
        var viewModel = result.ViewData.Model as LargeProducerRegisterViewModel;

        // Assert
        viewModel.RegisterOfProducers.Should().ContainSingle()
            .Which.DownloadUrl.Should().Be($"/public-register/report?fileName=2025/producers.csv&type={TestContainerName}");
    }

    [TestMethod]
    public async Task Get_RegisterOfProducers_QueriesBlobStorageWithFolderPrefixesFrom2025ToPreviousYear()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year;
        var previousYear = currentYear - 1;

        _largeProducerRegisterService
            .Setup(x => x.GetLatestAllNationsFileInfoAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<LargeProducerFileInfoViewModel>());

        // Act
        await _systemUnderTest.Get();

        // Assert
        _blobStorageService.Verify(x => x.GetLatestFilePropertiesAsync(
            TestContainerName,
            It.Is<List<string>>(prefixes =>
                prefixes.All(p => int.Parse(p) >= 2025) &&
                prefixes.All(p => int.Parse(p) <= previousYear) &&
                !prefixes.Contains(currentYear.ToString()))),
            Times.Once);
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