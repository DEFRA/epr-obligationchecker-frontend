namespace FrontendObligationChecker.UnitTests.Controllers;

using System.Text;
using Constants;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using Microsoft.AspNetCore.Http;
using Moq;

[TestClass]
public class CultureControllerTests
{
    private const string ReturnUrl = "returnUrl";
    private const string CultureEn = Language.English;
    private Mock<IResponseCookies>? _responseCookiesMock;
    private Mock<ISession> _sessionMock;
    private Mock<HttpContext>? _httpContextMock;
    private CultureController? _systemUnderTest;

    [TestInitialize]
    public void Setup()
    {
        _responseCookiesMock = new Mock<IResponseCookies>();
        _sessionMock = new Mock<ISession>();
        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock.Setup(x => x.Session).Returns(_sessionMock.Object);
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

        var cultureBytes = Encoding.UTF8.GetBytes(CultureEn);

        // Act
        var result = _systemUnderTest!.UpdateCulture(CultureEn, ReturnUrl);

        // Assert
        result.Url.Should().Be(ReturnUrl);
        _sessionMock.Verify(x => x.Set(Language.SessionLanguageKey, cultureBytes), Times.Once);
    }
}