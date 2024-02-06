namespace FrontendObligationChecker.UnitTests.Readers;

using Azure;
using Azure.Storage.Blobs;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Readers;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
public class BlobReaderTests
{
    private Mock<BlobContainerClient> _blobContainerClient;
    private Mock<BlobClient> _blobClient;
    private Mock<ILogger<BlobReader>> _logger;

    private BlobReader _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _blobContainerClient = new Mock<BlobContainerClient>();
        _blobClient = new Mock<BlobClient>();
        _logger = new Mock<ILogger<BlobReader>>();

        _systemUnderTest = new BlobReader(_blobContainerClient.Object, _logger.Object);
    }

    [TestMethod]
    public async Task DownloadBlobToStreamAsync_BlobContainerFindsFile_ReturnsMemoryStream()
    {
        // Arrange
        const string fileName = "fileName";

        _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClient.Object);

        // Act
        var result = await _systemUnderTest.DownloadBlobToStreamAsync(fileName);

        // Assert
        result.Should().BeOfType<MemoryStream>();
    }

    [TestMethod]
    public async Task DownloadBlobToStreamAsync_BlobContainerClientThrowsException_ThrowsRequestFailedException()
    {
        // Arrange
        const string logMessage = "Failed to read {FileName} from blob storage";
        const string fileName = "fileName";

        _blobClient.Setup(x => x.DownloadToAsync(It.IsAny<Stream>()))
            .ThrowsAsync(new RequestFailedException(It.IsAny<string>()));
        _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClient.Object);

        // Act
        var act = () => _systemUnderTest.DownloadBlobToStreamAsync(fileName);

        // Assert
        act.Should().ThrowAsync<BlobReaderException>();
        _logger.VerifyLog(logger => logger.LogError(logMessage, fileName), Times.Once);
    }
}