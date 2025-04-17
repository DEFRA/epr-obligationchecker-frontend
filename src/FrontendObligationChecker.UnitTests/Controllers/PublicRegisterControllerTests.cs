﻿namespace FrontendObligationChecker.UnitTests.Controllers
{
    using System.Globalization;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FrontendObligationChecker.Controllers;
    using FrontendObligationChecker.Models.BlobReader;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Services.PublicRegister;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PublicRegisterControllerTests
    {
        private Mock<IBlobStorageService> _blobStorageServiceMock = null!;
        private IOptions<PublicRegisterOptions> _options = null!;
        private PublicRegisterController _controller = null!;
        private DateTime _publishedDate;
        private DateTime _lastModified;

        [TestInitialize]
        public void Setup()
        {
            _blobStorageServiceMock = new Mock<IBlobStorageService>();

            _publishedDate = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc);
            _lastModified = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc);

            var producerBlob = new PublicRegisterBlobModel
            {
                Name = "producers.csv",
                PublishedDate = _publishedDate,
                LastModified = _lastModified,
                ContentLength = "115",
                FileType = "text/csv"
            };

            var complianceBlob = new PublicRegisterBlobModel
            {
                Name = "schemes.csv",
                PublishedDate = _publishedDate,
                LastModified = _lastModified,
                ContentLength = "450",
                FileType = "text/csv"
            };

            _blobStorageServiceMock
                .Setup(x => x.GetLatestFilePropertiesAsync("producers-container"))
                .ReturnsAsync(producerBlob);

            _blobStorageServiceMock
                .Setup(x => x.GetLatestFilePropertiesAsync("schemes-container"))
                .ReturnsAsync(complianceBlob);

            _options = Options.Create(new PublicRegisterOptions
            {
                PublicRegisterBlobContainerName = "producers-container",
                PublicRegisterCsoBlobContainerName = "schemes-container"
            });

            _controller = new PublicRegisterController(_blobStorageServiceMock.Object, _options);
        }

        [TestMethod]
        public async Task Guidance_ReturnsExpectedViewWithCorrectModel()
        {
            // Act
            var result = await _controller.Guidance();

            // Assert
            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult!.ViewName.Should().Be("Guidance");
            viewResult.Model.Should().BeOfType<GuidanceViewModel>();

            var model = (GuidanceViewModel)viewResult.Model!;

            var expectedDate = _publishedDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
            var expectedLastUpdated = _lastModified.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);

            model.PublishedDate.Should().Be(expectedDate);
            model.LastUpdated.Should().Be(expectedLastUpdated);

            model.ProducerRegisteredFile.Should().NotBeNull();
            model.ProducerRegisteredFile!.FileName.Should().Be("producers.csv");
            model.ProducerRegisteredFile.FileSize.Should().Be("115");
            model.ProducerRegisteredFile.FileType.Should().Be("text/csv");
            model.ProducerRegisteredFile.DatePublished.Should().Be(expectedDate);
            model.ProducerRegisteredFile.DateLastModified.Should().Be(expectedLastUpdated);

            model.ComplianceSchemeRegisteredFile.Should().NotBeNull();
            model.ComplianceSchemeRegisteredFile!.FileName.Should().Be("schemes.csv");
            model.ComplianceSchemeRegisteredFile.FileSize.Should().Be("450");
            model.ComplianceSchemeRegisteredFile.FileType.Should().Be("text/csv");
            model.ComplianceSchemeRegisteredFile.DatePublished.Should().Be(expectedDate);
            model.ComplianceSchemeRegisteredFile.DateLastModified.Should().Be(expectedLastUpdated);
        }
    }
}