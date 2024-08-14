namespace FrontendObligationChecker.UnitTests.Controllers;

using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder.Interfaces;
using FrontendObligationChecker.Services.PageService.Interfaces;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class ObligationCheckerControllerTests
{
    private Mock<ILogger<ObligationCheckerController>>? _loggerMock;
    private Mock<IPageService>? _pageServiceMock;
    private Mock<INextFinder>? _nextFinderMock;
    private Mock<HttpContext>? _httpContextMock;
    private Mock<HttpRequest> _httpRequestMock;
    private ObligationCheckerController? _systemUnderTest;
    private Mock<IConfiguration>? _configurationMock;
    private Mock<IOptions<SiteDateOptions>>? _siteDateOptions;

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerMock = new Mock<ILogger<ObligationCheckerController>>();
        _pageServiceMock = new Mock<IPageService>();
        _httpContextMock = new Mock<HttpContext>();
        _httpRequestMock = new Mock<HttpRequest>();
        _configurationMock = new Mock<IConfiguration>();
        _siteDateOptions = new Mock<IOptions<SiteDateOptions>>();

        _systemUnderTest = new ObligationCheckerController(
            _loggerMock.Object,
            _pageServiceMock.Object,
            _configurationMock.Object,
            _siteDateOptions.Object);

        _systemUnderTest.ControllerContext.HttpContext = _httpContextMock.Object;
        _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
        _nextFinderMock = new Mock<INextFinder>();
    }

    [TestMethod]
    [DataRow(PagePath.TypeOfOrganisation)]
    public async Task GetQuestion_OnSuccess_ReturnsCorrectViewAndModel(string path)
    {
        // Arrange
        var page = GetPage(path);
        var expectedViewName = page.View;
        var expectedViewModel = new PageModel(page)
        {
            CurrentPage = "~/type-of-organisation?lang=en",
        };

        _pageServiceMock!.Setup(x => x.GetPageAsync(path))
            .Returns(Task.FromResult(page));

        _httpRequestMock.Setup(x => x.QueryString).Returns(new QueryString("?lang=en"));
        _httpRequestMock.Setup(x=>x.Path).Returns(new PathString("/type-of-organisation"));

        // Act
        var result = await _systemUnderTest!.Question(path) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.ViewName.Should().Be(expectedViewName);
        result.Model.Should().BeOfType(typeof(PageModel));
        result.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [TestMethod]
    public async Task GetQuestion_404_WhenGetPageReturnsNull()
    {
        // Arrange
        _pageServiceMock.Setup(x => x.GetPageAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<Page>(null));

        // Act
        var result = await _systemUnderTest!.Question(string.Empty);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetNextPage_RedirectsToOrganisationTypePath_WhenSetAnswersAndGetPageReturnsNull()
    {
        // Arrange
        var expectedRedirectPath = PagePath.TypeOfOrganisation;
        _pageServiceMock.Setup(x => x.SetAnswersAndGetPageAsync(
                It.IsAny<string>(),
                It.IsAny<IFormCollection>()))
            .Returns(Task.FromResult<Page>(null));

        _httpContextMock!
            .Setup(x => x.Request.Form)
            .Returns(new FormCollection(null));

        // Act
        var result = await _systemUnderTest!.GetNextPage(string.Empty) as RedirectResult;

        // Assert
        result.Url.Should().Be(expectedRedirectPath);
    }

    [TestMethod]
    [DataRow(PagePath.TypeOfOrganisation)]
    public async Task GetNextPage_OnSuccess_ReturnsRedirectResult(string path)
    {
        // Arrange
        var page = GetPage(path);
        _pageServiceMock!
            .Setup(x => x.SetAnswersAndGetPageAsync(
                It.IsAny<string>()
                , It.IsAny<IFormCollection>()))
            .Returns(Task.FromResult(page));

        _httpContextMock!
            .Setup(x => x.Request.Form)
            .Returns(new FormCollection(null));

        _nextFinderMock!
            .Setup(x => x.Next(It.IsAny<IList<Question>>()))
            .Returns(OptionPath.Primary);

        _systemUnderTest.Url = Mock.Of<IUrlHelper>(helper => helper.RouteUrl(It.IsAny<UrlRouteContext>()) == "/");

        // Act
        var result = await _systemUnderTest!.GetNextPage(path);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(RedirectResult));
    }

    private Page GetPage(string path)
    {
        return new Page()
        {
            NextFinder = _nextFinderMock!.Object,
            Path = path,
        };
    }
}