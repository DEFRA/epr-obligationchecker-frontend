namespace FrontendObligationChecker.UnitTests.Services.PublicRegister
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using FluentAssertions;
    using FrontendObligationChecker.Exceptions;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Readers;
    using FrontendObligationChecker.Services.PublicRegister;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class BlobStorageServiceTests
    {
        private Mock<BlobServiceClient> _blobServiceClientMock;
        private Mock<BlobContainerClient> _containerClientMock;
        private Mock<BlobClient> _blobClientMock;
        private Mock<IOptions<PublicRegisterOptions>> _optionsMock;
        private Mock<ILogger<BlobStorageService>> _loggerMock;
        private Mock<ILogger<BlobReader>> _blobReaderloggerMock;
        private BlobStorageService _service;
        private Mock<BlobReader> _blobReaderMock;

        [TestInitialize]
        public void Setup()
        {
            _blobServiceClientMock = new Mock<BlobServiceClient>();
            _containerClientMock = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();
            _optionsMock = new Mock<IOptions<PublicRegisterOptions>>();
            _loggerMock = new Mock<ILogger<BlobStorageService>>();
            _blobReaderloggerMock = new Mock<ILogger<BlobReader>>();

            _optionsMock.Setup(x => x.Value).Returns(new PublicRegisterOptions
            {
                PublicRegisterBlobContainerName = "test-container",
                PublishedDate = DateTime.UtcNow
            });

            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_containerClientMock.Object);

            _blobReaderMock = new Mock<BlobReader>(_containerClientMock.Object, _blobServiceClientMock.Object, _blobReaderloggerMock.Object);

            _service = new BlobStorageService(_blobServiceClientMock.Object, _blobReaderMock.Object, _loggerMock.Object, _optionsMock.Object);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_DoesNotReturnNull(string containerName)
        {
            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns((BlobContainerClient)null);

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_ReturnsOnlyPublicRegisterBlobModelWithPublishedDate_WhenContainerClientIsNull(string containerName)
        {
            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns((BlobContainerClient)null);

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PublishedDate);
            Assert.IsNull(result.LastModified);
            Assert.IsNull(result.ContentLength);
            Assert.IsNull(result.FileType);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_ReturnsOnlyPublicRegisterBlobModelWithPublishedDate_WhenNoBlobsFound(string containerName)
        {
            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobList = new List<BlobItem>();

            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x =>
                x.GetBlobsAsync(
                    It.IsAny<BlobTraits>(),
                    It.IsAny<BlobStates>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PublishedDate);
            Assert.IsNull(result.LastModified);
            Assert.IsNull(result.ContentLength);
            Assert.IsNull(result.FileType);
        }

        [TestMethod]
        [DataRow("ProducerContainer", "")]
        [DataRow("ComplianceSchemeContainer", ".CSV")]
        public async Task GetLatestFilePropertiesAsync_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties(string containerName, string fileType)
        {
            var mockResponse = new Mock<Response<BlobProperties>>();

            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobProperties = BlobsModelFactory.BlobProperties(lastModified: DateTimeOffset.UtcNow, contentLength: 1024, contentType: fileType);
            var blobList = new List<BlobItem>
                {
                    BlobsModelFactory.BlobItem($"2025/Blob1{fileType}"),
                    BlobsModelFactory.BlobItem($"2026/Blob2{fileType}"),
                    BlobsModelFactory.BlobItem($"Blob3{fileType}")
                };
            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            var metadata = new Dictionary<string, string>() { { "ContentLength", "180" } };
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(x => x.SetMetadata(metadata, null, It.IsAny<CancellationToken>()));
            mockResponse.Setup(r => r.Value).Returns(blobProperties);
            blobClientMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse.Object);

            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()).GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result.LastModified);
            Assert.IsNotNull(result.ContentLength);
            Assert.IsNotNull(result.FileType);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_ThrowRequestFailedException_WhenFailsGetBlobs(string containerName)
        {
            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobList = new List<BlobItem>();

            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new RequestFailedException(string.Format("Failed to read {0} from blob storage", "directories")));

            var act = async () => await _service.GetLatestFilePropertiesAsync(containerName);

            act.Should().ThrowAsync<BlobReaderException>();
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_ThrowRequestFailedException_WhenFailsGetBlobClient(string containerName)
        {
            var blobProperties = BlobsModelFactory.BlobProperties(lastModified: DateTimeOffset.UtcNow, contentLength: 1024);
            var blobList = new List<BlobItem>
                {
                    BlobsModelFactory.BlobItem("2025/Blob1"),
                };
            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            var blobClientMock = new Mock<BlobClient>();
            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()).GetBlobClient(It.IsAny<string>()))
                                  .Throws(new RequestFailedException("Failed to read public register producers files from blob storage"));

            var act = async () => await _service.GetLatestFilePropertiesAsync(containerName);

            act.Should().ThrowAsync<BlobReaderException>();
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFileAsync_ThrowRequestFailedException_WhenFailsGetBlobs(string containerName)
        {
            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobList = new List<BlobItem>();

            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new RequestFailedException(string.Format("Failed to read {0} from blob storage", "directories")));

            var act = async () => await _service.GetLatestFileAsync(containerName);

            act.Should().ThrowAsync<BlobReaderException>();
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFileAsync_ThrowRequestFailedException_WhenFailsGetBlobClient(string containerName)
        {
            var blobProperties = BlobsModelFactory.BlobProperties(lastModified: DateTimeOffset.UtcNow, contentLength: 1024);
            var blobList = new List<BlobItem>
                {
                    BlobsModelFactory.BlobItem("2025/Blob1"),
                };
            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            var blobClientMock = new Mock<BlobClient>();
            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()).GetBlobClient(It.IsAny<string>()))
                                  .Throws(new RequestFailedException("Failed to read public register producers files from blob storage"));

            var act = async () => await _service.GetLatestFileAsync(containerName);

            act.Should().ThrowAsync<BlobReaderException>();
        }

        [TestMethod]
        [DataRow("ProducerContainer", "")]
        [DataRow("ComplianceSchemeContainer", ".CSV")]
        public async Task GetLatestFileAsync_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties(string containerName, string fileType)
        {
            var mockResponse = new Mock<Response<BlobProperties>>();
            var mockDownloadResponse = new Mock<Response<BlobDownloadResult>>();
            var downloadContent = BlobsModelFactory.BlobDownloadResult(content: new BinaryData("some data"));

            var mockBlobContainerClient = new Mock<BlobContainerClient>();
            var blobProperties = BlobsModelFactory.BlobProperties(lastModified: DateTimeOffset.UtcNow, contentLength: 1024, contentType: fileType);
            var mockValue = BlobsModelFactory.BlobDownloadResult(new BinaryData("some data"), default);
            var blobContent = new BinaryData("this is test data");
            var downloadResult = BlobsModelFactory.BlobDownloadResult(content: blobContent);
            var response = Response.FromValue(downloadResult, new Mock<Response>().Object);
            var blobList = new List<BlobItem>
                {
                    BlobsModelFactory.BlobItem($"2025/Blob1{fileType}"),
                    BlobsModelFactory.BlobItem($"2026/Blob2{fileType}"),
                    BlobsModelFactory.BlobItem($"Blob3{fileType}")
                };
            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            var metadata = new Dictionary<string, string>() { { "ContentLength", "180" } };
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(x => x.SetMetadata(metadata, null, It.IsAny<CancellationToken>()));
            mockResponse.Setup(r => r.Value).Returns(blobProperties);
            mockDownloadResponse.Setup(r => r.Value).Returns(downloadContent);
            blobClientMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse.Object);
            blobClientMock.Setup(x => x.DownloadContentAsync()).ReturnsAsync(mockDownloadResponse.Object);
            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()).GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            var result = await _service.GetLatestFileAsync(containerName);

            result.FileName.Equals("Blob1");
            Assert.IsNotNull(result.FileContent);
        }

        [TestMethod]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folders_Return_NoData()
        {
            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobList = new List<BlobItem>();

            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x =>
                x.GetBlobsAsync(
                    It.IsAny<BlobTraits>(),
                    It.IsAny<BlobStates>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            Dictionary<string, PublicRegisterBlobModel> result =
                await _service.GetLatestFilePropertiesAsync("ProducerContainer", new List<string> { DateTime.UtcNow.Year.ToString() });

            Assert.IsNotNull(result);
            result.Count.Should().Be(0);
        }

        [TestMethod]
        [DataRow("ProducerContainer", "")]
        [DataRow("ProducerContainer", ".CSV")]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folder_2025_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties(string containerName, string fileType)
        {
            Mock<BlobClient> blobClientMock = SetupMockDataForGetLatestFileProperties(fileType);

            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()).GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            var result = await _service.GetLatestFilePropertiesAsync(containerName, new List<string> { DateTime.UtcNow.Year.ToString() });
            var hasKey = result.ContainsKey(DateTime.UtcNow.Year.ToString());
            var publicRegisterBlobModel = result[DateTime.UtcNow.Year.ToString()];

            Assert.IsNotNull(result);
            result.Count.Should().Be(1);
            hasKey.Should().BeTrue();
            Assert.IsNotNull(publicRegisterBlobModel.LastModified);
            publicRegisterBlobModel.ContentLength.Should().NotBeNull();
            publicRegisterBlobModel.FileType.Should().NotBeNull();
            publicRegisterBlobModel.Name.Should().Contain("Blob1");
        }

        [TestMethod]
        [DataRow("ProducerContainer", "")]
        [DataRow("ProducerContainer", ".CSV")]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folders_2025_And_2026_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties(string containerName, string fileType)
        {
            Mock<BlobClient> blobClientMock = SetupMockDataForGetLatestFileProperties(fileType);

            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()).GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            var result = await _service.GetLatestFilePropertiesAsync(containerName, new List<string> { DateTime.UtcNow.Year.ToString(), (DateTime.UtcNow.Year + 1).ToString() });
            var hasKey = result.ContainsKey(DateTime.UtcNow.Year.ToString());
            var publicRegisterBlobModel = result[DateTime.UtcNow.Year.ToString()];

            Assert.IsNotNull(result);
            result.Count.Should().Be(2);
            hasKey.Should().BeTrue();
            Assert.IsNotNull(publicRegisterBlobModel.LastModified);
            publicRegisterBlobModel.ContentLength.Should().NotBeNull();
            publicRegisterBlobModel.FileType.Should().NotBeNull();
            publicRegisterBlobModel.Name.Should().Contain("Blob1");
        }

        private Mock<BlobClient> SetupMockDataForGetLatestFileProperties(string fileType)
        {
            string currentYear = DateTime.UtcNow.Year.ToString();
            string nextYear = (DateTime.UtcNow.Year + 1).ToString();

            var mockResponse = new Mock<Response<BlobProperties>>();

            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobProperties = BlobsModelFactory.BlobProperties(lastModified: DateTimeOffset.UtcNow, contentLength: 1024, contentType: fileType);
            var blobList = new List<BlobItem>
                {
                    BlobsModelFactory.BlobItem($"{currentYear}/Blob1{fileType}"),
                    BlobsModelFactory.BlobItem($"{nextYear}/Blob2{fileType}"),
                    BlobsModelFactory.BlobItem($"Blob3{fileType}")
                };
            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            var metadata = new Dictionary<string, string>() { { "ContentLength", "180" } };
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(x => x.SetMetadata(metadata, null, It.IsAny<CancellationToken>()));
            mockResponse.Setup(r => r.Value).Returns(blobProperties);
            blobClientMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse.Object);
            return blobClientMock;
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        public async Task GetLatestFileAsync_For_Given_Folders_ThrowRequestFailedException_WhenFailsGetBlobs(string containerName)
        {
            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobList = new List<BlobItem>();

            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new RequestFailedException(string.Format("Failed to read {0} from blob storage", "directories")));

            var act = async () => await _service.GetLatestFilePropertiesAsync(containerName, new List<string> { DateTime.UtcNow.Year.ToString() });

            act.Should().ThrowAsync<BlobReaderException>();

        }
    }
}