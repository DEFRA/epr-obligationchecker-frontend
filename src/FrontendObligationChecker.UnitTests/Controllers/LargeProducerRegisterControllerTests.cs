namespace FrontendObligationChecker.UnitTests.Controllers;

using Constants;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
public class LargeProducerRegisterControllerTests
{
    private Mock<ILargeProducerRegisterService> _largeProducerRegisterService;
    private Mock<ILogger<LargeProducerRegisterController>> _logger;

    private LargeProducerRegisterController _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _largeProducerRegisterService = new Mock<ILargeProducerRegisterService>();
        _logger = new Mock<ILogger<LargeProducerRegisterController>>();

        _systemUnderTest = new LargeProducerRegisterController(_largeProducerRegisterService.Object, _logger.Object);
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
        const string expectedLog = "Failed to receive file for nation code {NationCode}";

        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>()))
            .ThrowsAsync(new LargeProducerRegisterServiceException());

        // Act
        var result = await _systemUnderTest.File(nationCode) as RedirectToActionResult;

        // Assert
        result.ActionName.Should().Be("Error");
        _logger.VerifyLog(logger => logger.LogError(expectedLog, nationCode), Times.Once);
    }

    [TestMethod]
    public async Task GetFile_InvalidNationCode_RedirectToLargeProducerErrorPage()
    {
        // Arrange
        const string nationCode = "XX";
        const string expectedLog = "Failed to receive file for nation code {NationCode}";

        _largeProducerRegisterService.Setup(x => x.GetReportAsync(It.IsAny<string>()))
            .ThrowsAsync(new LargeProducerRegisterServiceException());

        // Act
        var result = await _systemUnderTest.File(nationCode) as RedirectToActionResult;

        // Assert
        result.ActionName.Should().Be("Error");
        _logger.VerifyLog(logger => logger.LogError(expectedLog, nationCode), Times.Once);
    }

    [TestMethod]
    public async Task GetError_ReturnsLargeProducerErrorView_WhenCalled()
    {
        // Arrange

        // Act
        var result = await _systemUnderTest.Error() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("LargeProducerError");
    }
}