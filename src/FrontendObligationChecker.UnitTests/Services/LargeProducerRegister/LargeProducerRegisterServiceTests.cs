namespace FrontendObligationChecker.UnitTests.Services.LargeProducerRegister;

using Constants;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Readers;
using FrontendObligationChecker.Services.LargeProducerRegister;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class LargeProducerRegisterServiceTests
{
    private Mock<IBlobReader> _blobReaderMock;
    private Mock<ILogger<LargeProducerRegisterService>> _logger;

    private LargeProducerRegisterService _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _blobReaderMock = new Mock<IBlobReader>();
        _logger = new Mock<ILogger<LargeProducerRegisterService>>();
        _systemUnderTest = new LargeProducerRegisterService(_blobReaderMock.Object,
            Options.Create(
                new LargeProducerReportFileNamesOptions
                {
                    En = "en.csv",
                    Sc = "sc.csv",
                    Wl = "wl.csv",
                    Ni = "ni.csv",
                    All = "all.csv"
                }),
            _logger.Object);
    }

    [DataRow(HomeNation.England)]
    [DataRow(HomeNation.Scotland)]
    [DataRow(HomeNation.Wales)]
    [DataRow(HomeNation.NorthernIreland)]
    [DataRow(HomeNation.All)]
    [TestMethod]
    public async Task GetReportAsync_ReturnsMemoryStream(string nationCode)
    {
        // Arrange
        var expected = new MemoryStream();
        _blobReaderMock.Setup(x => x.DownloadBlobToStreamAsync(It.IsAny<string>())).ReturnsAsync(expected);

        // Act
        var result = await _systemUnderTest.GetReportAsync(nationCode);

        // Assert
        _blobReaderMock.Verify(x => x.DownloadBlobToStreamAsync(It.IsAny<string>()), Times.Once);
        result.Should().BeOfType<(Stream, string)>();
    }

    [TestMethod]
    public async Task GetReportAsync_BlobReaderThrowsBlobReaderException_ThrowLargeProducerRegisterServiceException()
    {
        // Arrange
        const string nationCode = HomeNation.England;
        const string logMessage = "Failed to get report for nation code {NationCode}";
        _blobReaderMock.Setup(x => x.DownloadBlobToStreamAsync(It.IsAny<string>())).ThrowsAsync(new BlobReaderException());

        // Act
        var act = () => _systemUnderTest.GetReportAsync(nationCode);

        // Assert
        act.Should().ThrowAsync<LargeProducerRegisterServiceException>();
        _logger.VerifyLog(logger => logger.LogError(logMessage, nationCode), Times.Once);

    }
}