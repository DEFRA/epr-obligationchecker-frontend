namespace FrontendObligationChecker.UnitTests.Controllers
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using FrontendObligationChecker.Controllers;
    using FrontendObligationChecker.Models.Config;
    using FrontendObligationChecker.Sessions;
    using FrontendObligationChecker.ViewModels.PublicRegister;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PublicRegisterControllerTests
    {
        private Mock<SessionRequestCultureProvider> _mockCultureProvider;
        private Mock<HttpContext>? _mockHttpContext;
        private Mock<ISession> _mockSession;
        private PublicRegisterController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCultureProvider = new Mock<SessionRequestCultureProvider>();
            _mockSession = new Mock<ISession>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpContext.Setup(mock => mock.Session).Returns(_mockSession.Object);
            var externalUrlsOptions = Options.Create(new ExternalUrlsOptions
            {
                DefraUrl = "https://www.defraurl.com"
            });
            _controller = new PublicRegisterController(externalUrlsOptions);
            _controller.ControllerContext.HttpContext = _mockHttpContext.Object;
        }

        [TestMethod]
        public async Task PublicRegisterGuidance_ShouldReturnView_WithCorrectViewModel()
        {
            // Arrange
            var expectedViewModel = new GuidanceViewModel
            {
                ComplianceSchemesRegisteredFileSize = "450",
                LastUpdated = "10 March 2025",
                ProducersRegisteredFileSize = "115",
                PublishedDate = "6 December 2025"
            };

            // Act
            var result = await _controller.Guidance() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual("Guidance", result.ViewName, "The view name should be 'Guidance'.");

            var model = result.Model as GuidanceViewModel;
            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(expectedViewModel, model.GetType());
            model.DefraUrl.Should().NotBeNullOrWhiteSpace();
        }
    }
}