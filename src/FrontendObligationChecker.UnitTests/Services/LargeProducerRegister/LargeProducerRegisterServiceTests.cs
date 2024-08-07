namespace FrontendObligationChecker.UnitTests.Services.LargeProducerRegister;

using Constants;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Readers;
using FrontendObligationChecker.Services.LargeProducerRegister;
using FrontendObligationChecker.ViewModels.LargeProducer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class LargeProducerRegisterServiceTests
{
    private const string _latestAllNationsReportFileNamePrefix = "prefix";
    private const string _latestAllNationsReportFileNamePrefixInWelsh = "prefixInWelsh";
    private const string _latestAllNationsReportDownloadFileName = "donwloadFileName.csv";
    private const string _latestAllNationsReportDownloadFileNameInWelsh = "donwloadFileNameInWelsh.csv";

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
                    EnglishReportFileName = "en.csv",
                    ScottishReportFileName = "sc.csv",
                    WalesReportFileName = "wl.csv",
                    NorthernIrelandReportFileName = "ni.csv",
                    AllNationsReportFileName = "all.csv",
                    LatestAllNationsReportDownloadFileName = _latestAllNationsReportDownloadFileName,
                    LatestAllNationsReportDownloadFileNameInWelsh = _latestAllNationsReportDownloadFileNameInWelsh,
                    LatestAllNationsReportFileNamePrefix = _latestAllNationsReportFileNamePrefix,
                    LatestAllNationsReportFileNamePrefixInWelsh = _latestAllNationsReportFileNamePrefixInWelsh,
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
        var result = await _systemUnderTest.GetReportAsync(nationCode, Language.English);

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
        var act = () => _systemUnderTest.GetReportAsync(nationCode, Language.English);

        // Assert
        act.Should().ThrowAsync<LargeProducerRegisterServiceException>();
        _logger.VerifyLog(logger => logger.LogError(logMessage, nationCode), Times.Once);
    }

    [TestMethod]
    public async Task GetAllReportFileSizesAsync_ReturnsCorrectDirectory()
    {
        // Arrange
        var expected = new Dictionary<string, string>()
        {
            {
                HomeNation.England, "1KB"
            },
            {
                HomeNation.Scotland, "2KB"
            },
            {
                HomeNation.Wales, "3KB"
            },
            {
                HomeNation.NorthernIreland, "4KB"
            },
            {
                HomeNation.All, "5KB"
            },
        };

        _blobReaderMock.Setup(x => x.GetFileSizeInBytesAsync("en.csv")).ReturnsAsync(1000);
        _blobReaderMock.Setup(x => x.GetFileSizeInBytesAsync("sc.csv")).ReturnsAsync(2000);
        _blobReaderMock.Setup(x => x.GetFileSizeInBytesAsync("wl.csv")).ReturnsAsync(3000);
        _blobReaderMock.Setup(x => x.GetFileSizeInBytesAsync("ni.csv")).ReturnsAsync(4000);
        _blobReaderMock.Setup(x => x.GetFileSizeInBytesAsync("all.csv")).ReturnsAsync(5000);

        // Act
        var result = await _systemUnderTest.GetAllReportFileSizesAsync(Language.English);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task GetAllReportFileSizesAsync_BlobReaderThrowsBlobReaderException_ThrowLargeProducerRegisterServiceException()
    {
        // Arrange
        const string logMessage = "Failed to get report metadata";
        _blobReaderMock.Setup(x => x.GetFileSizeInBytesAsync(It.IsAny<string>())).ThrowsAsync(new BlobReaderException());

        // Act
        var act = () => _systemUnderTest.GetAllReportFileSizesAsync(Language.English);

        // Assert
        act.Should().ThrowAsync<LargeProducerRegisterServiceException>();
        _logger.VerifyLog(logger => logger.LogError(logMessage), Times.Once);
    }

    [TestMethod]
    [DataRow(Language.English, _latestAllNationsReportFileNamePrefix)]
    [DataRow(Language.Welsh, _latestAllNationsReportFileNamePrefixInWelsh)]
    public async Task GetLatestAllNationsFileInfoAsync_ReturnsFileInfo(string culture, string expectedPrefix)
    {
        // Arrange
        var blobModels = new List<BlobModel>
        {
            new BlobModel
            {
                Name = "test",
                ContentLength = 100,
                CreatedOn = new DateTime(1970, 1, 1)
            },
            new BlobModel
            {
                Name = "test-2",
                ContentLength = 100,
                CreatedOn = new DateTime(1970, 1, 2)
            }
        };

        var expectedResult = new LargeProducerFileInfoViewModel
        {
            DisplayFileSize = "100B",
            DateCreated = blobModels[1].CreatedOn.Value
        };

        _blobReaderMock.Setup(x => x.GetBlobsAsync(expectedPrefix)).ReturnsAsync(blobModels);

        // Act
        var result = await _systemUnderTest.GetLatestAllNationsFileInfoAsync(culture);

        // Assert
        _blobReaderMock.Verify(x => x.GetBlobsAsync(expectedPrefix), Times.Once);
        result.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public async Task GetLatestAllNationsFileInfoAsync_ReturnsNull_IfNoBlobsFound()
    {
        // Arrange
        var blobModels = new List<BlobModel>();

        _blobReaderMock.Setup(x => x.GetBlobsAsync(_latestAllNationsReportFileNamePrefix)).ReturnsAsync(blobModels);

        // Act
        var result = await _systemUnderTest.GetLatestAllNationsFileInfoAsync(Language.English);

        // Assert
        _blobReaderMock.Verify(x => x.GetBlobsAsync(_latestAllNationsReportFileNamePrefix), Times.Once);
        result.Should().BeNull();
    }

    [TestMethod]
    [DataRow(Language.English, _latestAllNationsReportFileNamePrefix, _latestAllNationsReportDownloadFileName)]
    [DataRow(Language.Welsh, _latestAllNationsReportFileNamePrefixInWelsh, _latestAllNationsReportDownloadFileNameInWelsh)]
    public async Task GetLatestAllNationsFileAsync_ReturnsFile(string culture, string expectedPrefix, string downloadFileNamePattern)
    {
        // Arrange
        var blobModels = new List<BlobModel>
        {
            new BlobModel
            {
                Name = "test",
                ContentLength = 100,
                CreatedOn = new DateTime(1970, 1, 1)
            },
            new BlobModel
            {
                Name = "test-2",
                ContentLength = 100,
                CreatedOn = new DateTime(1970, 1, 2)
            }
        };

        var expectedResult = new LargeProducerFileViewModel
        {
            FileName = string.Format(downloadFileNamePattern, blobModels[1].CreatedOn),
            FileContents = new MemoryStream()
        };

        _blobReaderMock.Setup(x => x.GetBlobsAsync(expectedPrefix)).ReturnsAsync(blobModels);
        _blobReaderMock.Setup(x => x.DownloadBlobToStreamAsync(blobModels[1].Name)).ReturnsAsync(expectedResult.FileContents);

        // Act
        var result = await _systemUnderTest.GetLatestAllNationsFileAsync(culture);

        // Assert
        _blobReaderMock.Verify(x => x.GetBlobsAsync(expectedPrefix), Times.Once);
        _blobReaderMock.Verify(x => x.DownloadBlobToStreamAsync(blobModels[1].Name), Times.Once);
        result.Should().BeEquivalentTo(expectedResult);
    }

    [TestMethod]
    public async Task GetLatestAllNationsFileAsync_ReturnsNull_WhenNoBlobsFound()
    {
        // Arrange
        var blobModels = new List<BlobModel>();

        _blobReaderMock.Setup(x => x.GetBlobsAsync(_latestAllNationsReportFileNamePrefix)).ReturnsAsync(blobModels);

        // Act
        var result = await _systemUnderTest.GetLatestAllNationsFileAsync(Language.English);

        // Assert
        result.Should().BeNull();
        _blobReaderMock.Verify(x => x.GetBlobsAsync(_latestAllNationsReportFileNamePrefix), Times.Once);
        _blobReaderMock.Verify(x => x.DownloadBlobToStreamAsync(It.IsAny<string>()), Times.Never);
    }
}