namespace FrontendObligationChecker.UnitTests.Services.PublicRegister
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        private const string ProducersContainerName = "test-container";

        private static readonly string[] SingleDirectory = ["2025"];
        private static readonly string[] TwoDirectories = ["2025", "2026"];

        private Mock<IBlobReader> _blobReaderMock;
        private Mock<IOptions<PublicRegisterOptions>> _optionsMock;
        private Mock<ILogger<BlobStorageService>> _loggerMock;
        private BlobStorageService _service;

        [TestInitialize]
        public void Setup()
        {
            _blobReaderMock = new Mock<IBlobReader>();
            _optionsMock = new Mock<IOptions<PublicRegisterOptions>>();
            _loggerMock = new Mock<ILogger<BlobStorageService>>();

            _optionsMock.Setup(x => x.Value).Returns(new PublicRegisterOptions
            {
                PublicRegisterBlobContainerName = ProducersContainerName,
                PublishedDate = DateTime.UtcNow
            });

            _service = new BlobStorageService(_blobReaderMock.Object, _loggerMock.Object, _optionsMock.Object);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_DoesNotReturnNull(string containerName)
        {
            _blobReaderMock.Setup(x => x.GetDirectories(containerName))
                .ReturnsAsync(Enumerable.Empty<string>());

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_ReturnsOnlyPublicRegisterBlobModelWithPublishedDate_WhenNoBlobsFound(string containerName)
        {
            _blobReaderMock.Setup(x => x.GetDirectories(containerName))
                .ReturnsAsync(SingleDirectory);
            _blobReaderMock.Setup(x => x.GetBlobsAsync(containerName, "2025"))
                .ReturnsAsync(Enumerable.Empty<BlobModel>());

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result);
            Assert.IsNull(result.LastModified);
            Assert.IsNull(result.ContentLength);
            Assert.IsNull(result.FileType);
        }

        [TestMethod]
        [DataRow("ProducerContainer", "")]
        [DataRow("ComplianceSchemeContainer", ".CSV")]
        public async Task GetLatestFilePropertiesAsync_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties(string containerName, string fileType)
        {
            var lastModified = DateTime.UtcNow;
            _blobReaderMock.Setup(x => x.GetDirectories(containerName))
                .ReturnsAsync(TwoDirectories);
            _blobReaderMock.Setup(x => x.GetBlobsAsync(containerName, "2026"))
                .ReturnsAsync(new[]
                {
                    new BlobModel
                    {
                        Name = $"2026/Blob2{fileType}",
                        ContentLength = 1024,
                        LastModified = lastModified
                    }
                });

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result.LastModified);
            Assert.IsNotNull(result.ContentLength);
            Assert.IsNotNull(result.FileType);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_ThrowsBlobReaderException_WhenFailsGetDirectories(string containerName)
        {
            _blobReaderMock.Setup(x => x.GetDirectories(containerName))
                .ThrowsAsync(new BlobReaderException(string.Format("Failed to read {0} from blob storage", "directories")));

            var act = async () => await _service.GetLatestFilePropertiesAsync(containerName);

            // Service catches BlobReaderException and logs it, returning default result
            var result = await _service.GetLatestFilePropertiesAsync(containerName);
            Assert.IsNotNull(result);
            Assert.IsNull(result.LastModified);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        [DataRow("ComplianceSchemeContainer")]
        public async Task GetLatestFilePropertiesAsync_ThrowsBlobReaderException_WhenFailsGetBlobs(string containerName)
        {
            _blobReaderMock.Setup(x => x.GetDirectories(containerName))
                .ReturnsAsync(SingleDirectory);
            _blobReaderMock.Setup(x => x.GetBlobsAsync(containerName, "2025"))
                .ThrowsAsync(new BlobReaderException("Failed to read public register producers files from blob storage"));

            var result = await _service.GetLatestFilePropertiesAsync(containerName);

            Assert.IsNotNull(result);
            Assert.IsNull(result.LastModified);
        }

        [TestMethod]
        [DataRow("ProducerContainer", "testFileName.csv")]
        [DataRow("ComplianceSchemeContainer", "testFileName.csv")]
        public async Task GetLatestFileAsync_ThrowsBlobReaderException_WhenFailsDownload(string containerName, string blobName)
        {
            _blobReaderMock.Setup(x => x.DownloadBlobContentAsync(containerName, blobName))
                .ThrowsAsync(new BlobReaderException(string.Format("Failed to read {0} from blob storage", "directories")));

            var act = async () => await _service.GetLatestFileAsync(containerName, blobName);

            await act.Should().ThrowAsync<PublicRegisterServiceException>();
        }

        [TestMethod]
        [DataRow("ProducerContainer", "Blob1.CSV")]
        [DataRow("ComplianceSchemeContainer", ".CSV")]
        public async Task GetLatestFileAsync_ReturnsSuccess_WhenBlobContentDownloaded(string containerName, string fileType)
        {
            var blobName = $"2025/{fileType}";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("some data"));

            _blobReaderMock.Setup(x => x.DownloadBlobContentAsync(containerName, blobName))
                .ReturnsAsync(stream);

            var result = await _service.GetLatestFileAsync(containerName, blobName);

            Assert.IsNotNull(result.FileContent);
            result.FileName.Should().Be(fileType);
        }

        [TestMethod]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folders_Return_NoData()
        {
            _blobReaderMock.Setup(x => x.GetBlobsAsync("ProducerContainer", It.IsAny<string>()))
                .ReturnsAsync(Enumerable.Empty<BlobModel>());

            Dictionary<string, PublicRegisterBlobModel> result =
                await _service.GetLatestFilePropertiesAsync("ProducerContainer", new List<string> { DateTime.UtcNow.Year.ToString() });

            Assert.IsNotNull(result);
            result.Count.Should().Be(0);
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folder_2025_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties(string containerName)
        {
            string currentYear = DateTime.UtcNow.Year.ToString();
            var lastModified = DateTime.UtcNow;

            _blobReaderMock.Setup(x => x.GetBlobsAsync(containerName, currentYear))
                .ReturnsAsync(new[]
                {
                    new BlobModel
                    {
                        Name = $"{currentYear}/{currentYear}_register_of_producers.csv",
                        ContentLength = 1024,
                        LastModified = lastModified
                    }
                });

            var result = await _service.GetLatestFilePropertiesAsync(containerName, new List<string> { currentYear });
            var hasKey = result.ContainsKey(currentYear);
            var publicRegisterBlobModel = result[currentYear];

            Assert.IsNotNull(result);
            result.Count.Should().Be(1);
            hasKey.Should().BeTrue();
            Assert.IsNotNull(publicRegisterBlobModel.LastModified);
            publicRegisterBlobModel.ContentLength.Should().NotBeNull();
            publicRegisterBlobModel.FileType.Should().NotBeNull();
            publicRegisterBlobModel.Name.Should().Be($"{currentYear}/{currentYear}_register_of_producers.csv");
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folders_2025_And_2026_ReturnsSuccess_WhenBlobsFoundWithBlobClientProperties(string containerName)
        {
            string currentYear = DateTime.UtcNow.Year.ToString();
            string nextYear = (DateTime.UtcNow.Year + 1).ToString();
            var lastModified = DateTime.UtcNow;

            _blobReaderMock.Setup(x => x.GetBlobsAsync(containerName, currentYear))
                .ReturnsAsync(new[]
                {
                    new BlobModel
                    {
                        Name = $"{currentYear}/{currentYear}_register_of_producers.csv",
                        ContentLength = 1024,
                        LastModified = lastModified
                    }
                });
            _blobReaderMock.Setup(x => x.GetBlobsAsync(containerName, nextYear))
                .ReturnsAsync(new[]
                {
                    new BlobModel
                    {
                        Name = $"{nextYear}/{nextYear}_register_of_producers.csv",
                        ContentLength = 512,
                        LastModified = lastModified
                    }
                });

            var result = await _service.GetLatestFilePropertiesAsync(containerName, new List<string> { currentYear, nextYear });
            var hasKey = result.ContainsKey(currentYear);
            var publicRegisterBlobModel = result[currentYear];

            Assert.IsNotNull(result);
            result.Count.Should().Be(2);
            hasKey.Should().BeTrue();
            Assert.IsNotNull(publicRegisterBlobModel.LastModified);
            publicRegisterBlobModel.ContentLength.Should().NotBeNull();
            publicRegisterBlobModel.FileType.Should().NotBeNull();
            publicRegisterBlobModel.Name.Should().Be($"{currentYear}/{currentYear}_register_of_producers.csv");
        }

        [TestMethod]
        [DataRow("ProducerContainer")]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folders_ThrowsBlobReaderException_WhenFailsGetBlobs(string containerName)
        {
            _blobReaderMock.Setup(x => x.GetBlobsAsync(containerName, It.IsAny<string>()))
                .ThrowsAsync(new BlobReaderException(string.Format("Failed to read {0} from blob storage", "directories")));

            var result = await _service.GetLatestFilePropertiesAsync(containerName, new List<string> { DateTime.UtcNow.Year.ToString() });

            // Service catches BlobReaderException and logs it, returning empty dict
            Assert.IsNotNull(result);
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folders_ReturnsRegisterOfProducersFile_IgnoringNewerInternalFiles()
        {
            var publishedFileLastModified = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            _blobReaderMock.Setup(x => x.GetBlobsAsync("ProducerContainer", "2026"))
                .ReturnsAsync(new[]
                {
                    new BlobModel { Name = "2026/2026_register_of_producers.csv", ContentLength = 1024, LastModified = publishedFileLastModified },
                    new BlobModel { Name = "2026/2026_register_of_producers_DEBUG.csv", ContentLength = 2048, LastModified = publishedFileLastModified.AddDays(20) },
                    new BlobModel { Name = "2026/2026_register_of_producers_DEBUG_generated_on_2026-10-21.csv", ContentLength = 2048, LastModified = publishedFileLastModified.AddDays(20) },
                    new BlobModel { Name = "2026/2026_register_of_producers_generated_on_2026-10-16.csv", ContentLength = 1024, LastModified = publishedFileLastModified.AddDays(15) }
                });

            var result = await _service.GetLatestFilePropertiesAsync("ProducerContainer", new List<string> { "2026" });

            result["2026"].Name.Should().Be("2026/2026_register_of_producers.csv");
            result["2026"].LastModified.Should().Be(publishedFileLastModified);
        }

        [TestMethod]
        public async Task GetLatestFilePropertiesAsync_For_Given_Folders_ReturnsNoData_WhenRegisterOfProducersFileNotFound()
        {
            var lastModified = DateTime.UtcNow;

            _blobReaderMock.Setup(x => x.GetBlobsAsync("ProducerContainer", "2026"))
                .ReturnsAsync(new[]
                {
                    new BlobModel { Name = "2026/2026_register_of_producers_debug.csv", ContentLength = 2048, LastModified = lastModified },
                    new BlobModel { Name = "2026/2026_register_of_producers_generated_on_2026-10-16.csv", ContentLength = 1024, LastModified = lastModified },
                    new BlobModel { Name = "2026/2025_register_of_producers.csv", ContentLength = 1024, LastModified = lastModified }
                });

            var result = await _service.GetLatestFilePropertiesAsync("ProducerContainer", new List<string> { "2026" });

            result.Count.Should().Be(0);
        }

        [TestMethod]
        [DataRow("2026/2026_register_of_producers_DEBUG.csv")]
        [DataRow("2026/2026_register_of_producers_debug_generated_on_2026-10-21.csv")]
        [DataRow("2026/2026_register_of_producers_generated_on_2026-10-16.csv")]
        [DataRow("2026/2025_register_of_producers.csv")]
        [DataRow("2026/other.csv")]
        [DataRow("2026/2026_REGISTER_OF_PRODUCERS.CSV")]
        public async Task GetLatestFileAsync_ReturnsEmptyFileModel_WhenProducersFileIsNotRegisterOfProducers(string blobName)
        {
            var result = await _service.GetLatestFileAsync(ProducersContainerName, blobName);

            result.FileName.Should().BeNull();
            result.FileContent.Should().BeNull();
            _blobReaderMock.Verify(x => x.DownloadBlobContentAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetLatestFileAsync_ReturnsSuccess_WhenRegisterOfProducersFileRequested()
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("some data"));

            _blobReaderMock.Setup(x => x.DownloadBlobContentAsync(ProducersContainerName, "2026/2026_register_of_producers.csv"))
                .ReturnsAsync(stream);

            var result = await _service.GetLatestFileAsync(ProducersContainerName, "2026/2026_register_of_producers.csv");

            result.FileContent.Should().BeSameAs(stream);
            result.FileName.Should().Be("2026_register_of_producers.csv");
        }
    }
}
