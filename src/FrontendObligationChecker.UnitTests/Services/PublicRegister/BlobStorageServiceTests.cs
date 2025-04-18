﻿namespace FrontendObligationChecker.UnitTests.Services.PublicRegister
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using FluentAssertions;
    using FrontendObligationChecker.Exceptions;
    using FrontendObligationChecker.Models.Config;
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
        private IBlobStorageService _service;

        [TestInitialize]
        public void Setup()
        {
            _blobServiceClientMock = new Mock<BlobServiceClient>();
            _containerClientMock = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();
            _optionsMock = new Mock<IOptions<PublicRegisterOptions>>();
            _loggerMock = new Mock<ILogger<BlobStorageService>>();

            _optionsMock.Setup(x => x.Value).Returns(new PublicRegisterOptions
            {
                PublicRegisterBlobContainerName = "test-container",
                PublishedDate = DateTime.UtcNow
            });

            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_containerClientMock.Object);

            _service = new BlobStorageService(_blobServiceClientMock.Object, _loggerMock.Object,_optionsMock.Object);
        }

        [TestMethod]
        public async Task GetLatestProducersFilePropertiesAsync_ReturnsNull_WhenContainerClientIsNull()
        {
            _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns((BlobContainerClient)null);

            var result = await _service.GetLatestProducersFilePropertiesAsync();

            Assert.IsNull(result.LastModified);
            Assert.IsNull(result.ContentLength);
        }

        [TestMethod]
        public async Task GetLatestProducersFilePropertiesAsync_ReturnsNull_WhenNoBlobsFound()
        {
            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobList = new List<BlobItem>();

            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            var result = await _service.GetLatestProducersFilePropertiesAsync();

            Assert.IsNull(result.LastModified);
            Assert.IsNull(result.ContentLength);
        }

        [TestMethod]
        public async Task GetLatestProducersFilePropertiesAsync_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties()
        {
            var mockResponse = new Mock<Response<BlobProperties>>();

            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobProperties = BlobsModelFactory.BlobProperties(lastModified: DateTimeOffset.UtcNow, contentLength: 1024);
            var blobList = new List<BlobItem>
                {
                    BlobsModelFactory.BlobItem("2025/Blob1"),
                    BlobsModelFactory.BlobItem("2026/Blob2"),
                    BlobsModelFactory.BlobItem("Blob3")
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

            var result = await _service.GetLatestProducersFilePropertiesAsync();

            Assert.IsNotNull(result.LastModified);
            Assert.IsNotNull(result.ContentLength);
        }

        [TestMethod]
        public async Task GetLatestProducersFilePropertiesAsync_ThrowRequestFailedException_WhenFailsGetBlobs()
        {
            var mockBlobContainerClient = new Mock<BlobContainerClient>();

            var blobList = new List<BlobItem>();

            var page = Page<BlobItem>.FromValues(blobList, null, Mock.Of<Response>());
            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page });

            _containerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new RequestFailedException(string.Format("Failed to read {0} from blob storage", "directories")));

            var act = async () => await _service.GetLatestProducersFilePropertiesAsync();

            act.Should().ThrowAsync<BlobReaderException>();
        }

        [TestMethod]
        public async Task GetLatestProducersFilePropertiesAsync_ThrowRequestFailedException_WhenFailsGetBlobClient()
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

            var act = async () => await _service.GetLatestProducersFilePropertiesAsync();

            act.Should().ThrowAsync<BlobReaderException>();
        }
    }
}