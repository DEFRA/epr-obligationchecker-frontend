namespace FrontendObligationChecker.UnitTests.Readers;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Readers;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
public class BlobReaderTests
{
    private Mock<BlobContainerClient> _blobContainerClient;
    private Mock<BlobClient> _blobClient;
    private Mock<BlobServiceClient> _blobServiceClient;
    private Mock<ILogger<BlobReader>> _logger;

    private BlobReader _systemUnderTest;

    [TestInitialize]
    public void TestInitialize()
    {
        _blobContainerClient = new Mock<BlobContainerClient>();
        _blobClient = new Mock<BlobClient>();
        _blobServiceClient = new Mock<BlobServiceClient>();
        _logger = new Mock<ILogger<BlobReader>>();

        _systemUnderTest = new BlobReader(_blobContainerClient.Object, _blobServiceClient.Object, _logger.Object);
    }

    [TestMethod]
    public async Task DownloadBlobToStreamAsync_BlobContainerFindsFile_ReturnsMemoryStreamAndDoesNotPrepend_WhenAsked()
    {
        // Arrange
        const string fileName = "fileName";

        _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClient.Object);

        // Act
        var result = await _systemUnderTest.DownloadBlobToStreamAsync(fileName, false);

        // Assert
        result.Should().BeOfType<MemoryStream>();
        result.Length.Should().Be(0L);
    }

    [TestMethod]
    public async Task DownloadBlobToStreamAsync_BlobContainerFindsFile_ReturnsMemoryStreamAndPrependsBOM_WhenAsked()
    {
        // Arrange
        const string fileName = "fileName";

        _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClient.Object);

        // Act
        var result = await _systemUnderTest.DownloadBlobToStreamAsync(fileName, true);

        // Assert
        result.Should().BeOfType<MemoryStream>();
        var actualByteStart = new byte[3];
        await result.ReadAsync(actualByteStart.AsMemory(0, 3), CancellationToken.None);
        actualByteStart[0].Should().Be(0xEF);
        actualByteStart[1].Should().Be(0xBB);
        actualByteStart[2].Should().Be(0xBF);
        result.Length.Should().Be(3L);
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

#pragma warning disable MSTEST0049
        _blobClient.Setup(x => x.DownloadToAsync(It.IsAny<Stream>()))
#pragma warning restore MSTEST0049
            .ThrowsAsync(new RequestFailedException(It.IsAny<string>()));
        _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClient.Object);

        // Act
        var act = () => _systemUnderTest.DownloadBlobToStreamAsync(fileName);

        // Assert
        act.Should().ThrowAsync<BlobReaderException>();
        _logger.VerifyLog(logger => logger.LogError(logMessage, fileName), Times.Once);
    }

    [TestMethod]
    public async Task GetFileSizeInBytesAsync_BlobContainerFindsFile_ReturnsBytes()
    {
        // Arrange
        const string fileName = "fileName";
        var responseMock = new Mock<Response>();
        var blobProperties = BlobsModelFactory.BlobProperties(contentLength: 100);
        _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClient.Object);
        _blobClient
            .Setup(m => m.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(blobProperties, responseMock.Object));

        // Act
        var result = await _systemUnderTest.GetFileSizeInBytesAsync(fileName);

        // Assert
        result.Should().Be(100);
    }

    [TestMethod]
    public async Task GetFileSizeInBytesAsync_BlobContainerClientThrowsException_ThrowsRequestFailedException()
    {
        // Arrange
        const string logMessage = "Failed to read {FileName} from blob storage";
        const string fileName = "fileName";
        _blobClient
            .Setup(m => m.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(It.IsAny<string>()));
        _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClient.Object);
        // Act
        var act = () => _systemUnderTest.GetFileSizeInBytesAsync(fileName);

        // Assert
        act.Should().ThrowAsync<BlobReaderException>();
        _logger.VerifyLog(logger => logger.LogError(logMessage, fileName), Times.Once);
    }

    [TestMethod]
    public async Task GetBlobsAsync_BlobContainerFindsFiles_ReturnsBlobs()
    {
        // Arrange
        const string Prefix = "t";

        var expectedBlobs = new BlobModel[]
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

        var blobList = new BlobItem[]
        {
            BlobsModelFactory.BlobItem(
                name: expectedBlobs[0].Name,
                properties: BlobsModelFactory.BlobItemProperties(
                    accessTierInferred: true,
                    createdOn: new DateTimeOffset(expectedBlobs[0].CreatedOn.Value),
                    contentLength: expectedBlobs[0].ContentLength)),
            BlobsModelFactory.BlobItem(
                name: expectedBlobs[1].Name,
                properties: BlobsModelFactory.BlobItemProperties(
                    accessTierInferred: true,
                    createdOn: new DateTimeOffset(expectedBlobs[1].CreatedOn.Value),
                    contentLength: expectedBlobs[1].ContentLength)),
        };

        var pageableBlobList = AsyncPageable<BlobItem>.FromPages(new[] { Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>()) });

        _blobContainerClient.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, Prefix, default)).Returns(pageableBlobList);

        // Act
        var result = await _systemUnderTest.GetBlobsAsync(Prefix);

        // Assert
        result.Should().BeEquivalentTo(expectedBlobs);
        _blobContainerClient.Verify(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, Prefix, default), Times.Once);
    }

    [TestMethod]
    public async Task GetBlobsAsync_BlobContainerThrowsException_ThrowsRequestFailedException()
    {
        // Arrange
        const string Prefix = "t";

        _blobContainerClient.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, Prefix, default))
            .Throws(new RequestFailedException(It.IsAny<string>()));

        // Act
        var act = () => _systemUnderTest.GetBlobsAsync(Prefix);

        // Assert
        act.Should().ThrowAsync<BlobReaderException>();
        _blobContainerClient.Verify(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, Prefix, default), Times.Once);
    }

    [TestMethod]
    public async Task GetDirectories_BlobContainerFindsFilesDirectories_ReturnsDirectoryList()
    {
        // Arrange
        var expected = new string[] { "1970", "1971" };

        var blobHierarchyItemList = new BlobHierarchyItem[]
        {
            BlobsModelFactory.BlobHierarchyItem(expected[0],  BlobsModelFactory.BlobItem(name: "test")),
            BlobsModelFactory.BlobHierarchyItem(expected[1],  BlobsModelFactory.BlobItem(name: "test")),
            BlobsModelFactory.BlobHierarchyItem(null,  BlobsModelFactory.BlobItem(name: "test")),
        };

        var pageableBlobList = AsyncPageable<BlobHierarchyItem>.FromPages(
            new[] { Page<BlobHierarchyItem>.FromValues(blobHierarchyItemList, null, Mock.Of<Response>()) });

        _blobContainerClient.Setup(x => x.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", null, default))
            .Returns(pageableBlobList);

        // Act
        var result = await _systemUnderTest.GetDirectories();

        // Assert
        result.Should().BeEquivalentTo(expected);
        _blobContainerClient.Verify(x => x.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", null, default), Times.Once);
    }

    [TestMethod]
    public async Task GetDirectories_BlobContainerThrowsException_ThrowsRequestFailedException()
    {
        // Arrange
        _blobContainerClient.Setup(x => x.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", null, default))
            .Throws(new RequestFailedException(It.IsAny<string>()));

        // Act
        var act = () => _systemUnderTest.GetDirectories();

        // Assert
        act.Should().ThrowAsync<BlobReaderException>();
        _blobContainerClient.Verify(x => x.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", null, default), Times.Once);
    }
}