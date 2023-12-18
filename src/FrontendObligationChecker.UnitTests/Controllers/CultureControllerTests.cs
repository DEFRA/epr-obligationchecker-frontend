using FluentAssertions;
using FrontendObligationChecker.Controllers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FrontendObligationChecker.UnitTests.Controllers;
[TestClass]
public class CultureControllerTests
{
    private const string ReturnUrl = "returnUrl";

    private const string CultureEn = "en";

    private Mock<IResponseCookies>? _responseCookiesMock;

    private Mock<HttpContext>? _httpContextMock;

    private CultureController? _systemUnderTest;

    [TestInitialize]
    public void Setup()
    {
        _responseCookiesMock = new Mock<IResponseCookies>();
        _httpContextMock = new Mock<HttpContext>();
        _systemUnderTest = new CultureController();
        _systemUnderTest.ControllerContext.HttpContext = _httpContextMock.Object;
    }

    [TestMethod]
    public void CultureController_UpdateCulture_RedirectsToReturnUrlWithCulture()
    {
        // Arrange
        _httpContextMock!
            .Setup(x => x.Response.Cookies)
            .Returns(_responseCookiesMock!.Object);

        // Act
        var result = _systemUnderTest!.UpdateCulture(CultureEn, ReturnUrl);

        // Assert
        var expectedUrl = $"{ReturnUrl}?culture={CultureEn}";
        result.Url.Should().Be(expectedUrl);
    }
}