using FluentAssertions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Models.Cookies;
using FrontendObligationChecker.Services.Infrastructure.Interfaces;
using FrontendObligationChecker.ViewComponents;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Moq;

namespace FrontendObligationChecker.UnitTests.ViewComponents;

[TestClass]
public class CookieBannerViewComponentTests
{
    private const string CookieName = ".epr_cookie_policy";
    private const string PATH = "/test";
    private const string QUERY = "?test=true";

    private Mock<HttpContext>? _httpContextMock;
    private Mock<HttpRequest> _httpRequest;
    private Mock<HttpResponse> _httpResponse;

    private CookieConsentState _cookieConsentState;

    private Mock<IOptions<EprCookieOptions>> _cookieOptions;
    private Mock<ICookieService> _cookieService;

    [TestInitialize]
    public void TestInitialize()
    {
        _httpContextMock = new Mock<HttpContext>();
        _httpRequest = new Mock<HttpRequest>();
        _httpRequest.Setup(x => x.Path).Returns(PATH);
        _httpRequest.Setup(x => x.QueryString).Returns(new QueryString(QUERY));
        _httpResponse = new Mock<HttpResponse>();

        _httpContextMock.Setup(x => x.Request).Returns(_httpRequest.Object);
        _httpContextMock.Setup(x => x.Response).Returns(_httpResponse.Object);

        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        var responseCookiesMock = new Mock<IResponseCookies>();
        _httpContextMock.Setup(ctx => ctx.Request.Cookies).Returns(requestCookiesMock.Object);
        _httpContextMock.Setup(ctx => ctx.Response.Cookies).Returns(responseCookiesMock.Object);

        _cookieConsentState = new CookieConsentState
        {
            CookieAcknowledgementRequired = true,
            CookiesAccepted = true,
            CookieExists = true
        };

        _cookieService = new Mock<ICookieService>();
        _cookieService
            .Setup(ap => ap.GetConsentState(It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()))
            .Returns(_cookieConsentState);

        var eprCookieOptions = new EprCookieOptions() { CookiePolicyCookieName = CookieName };
        _cookieOptions = new Mock<IOptions<EprCookieOptions>>();
        _cookieOptions
            .Setup(ap => ap.Value)
            .Returns(eprCookieOptions);
    }

    [TestMethod]
    public void Invoke_SetsModel_WhenNotOnCookiePage()
    {
        // Arrange
        var cookieConsentState = new CookieConsentState
        {
            CookieAcknowledgementRequired = true,
            CookiesAccepted = true,
            CookieExists = true
        };

        _cookieService
            .Setup(ap => ap.GetConsentState(It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()))
            .Returns(cookieConsentState);

        var routeData = new Microsoft.AspNetCore.Routing.RouteData();
        routeData.Values.Add("controller", "Test");
        var component = new CookieBannerViewComponent(_cookieOptions.Object, _cookieService.Object)
        {
            ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = _httpContextMock.Object,
                    RouteData = routeData
                }
            }
        };

        // Act
        var result = component.Invoke() as ViewViewComponentResult;

        // Assert
        result.ViewData.Model.Should().BeEquivalentTo(new CookieBannerModel
        {
            ShowBanner = false,
            ShowAcknowledgement = true,
            AcceptAnalytics = true,
            ReturnUrl = $"~{PATH}{QUERY}"
        });
    }

    [TestMethod]
    public void Invoke_SetsModel_WhenNotOnCookiePage_And_CookieConsentStateAllFalse()
    {
        // Arrange
        var cookieConsentState = new CookieConsentState
        {
            CookieAcknowledgementRequired = false,
            CookiesAccepted = false,
            CookieExists = false
        };

        _cookieService
            .Setup(ap => ap.GetConsentState(It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()))
            .Returns(cookieConsentState);

        var routeData = new Microsoft.AspNetCore.Routing.RouteData();
        routeData.Values.Add("controller", "Test");
        var component = new CookieBannerViewComponent(_cookieOptions.Object, _cookieService.Object)
        {
            ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = _httpContextMock.Object,
                    RouteData = routeData
                }
            }
        };

        // Act
        var result = component.Invoke() as ViewViewComponentResult;

        // Assert
        result.ViewData.Model.Should().BeEquivalentTo(new CookieBannerModel
        {
            ShowBanner = true,
            ShowAcknowledgement = false,
            AcceptAnalytics = false,
            ReturnUrl = $"~{PATH}{QUERY}"
        });
    }

    [TestMethod]
    public void Invoke_SetsModel_WhenOnCookiePage()
    {
        // Arrange
        var cookieConsentState = new CookieConsentState
        {
            CookieAcknowledgementRequired = true,
            CookiesAccepted = true,
            CookieExists = true
        };

        _cookieService
            .Setup(ap => ap.GetConsentState(It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()))
            .Returns(cookieConsentState);

        var routeData = new Microsoft.AspNetCore.Routing.RouteData();
        routeData.Values.Add("controller", "Cookies");

        var component = new CookieBannerViewComponent(_cookieOptions.Object, _cookieService.Object)
        {
            ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = _httpContextMock.Object,
                    RouteData = routeData
                }
            }
        };

        // Act
        var result = component.Invoke() as ViewViewComponentResult;

        // Assert
        result.ViewData.Model.Should().BeEquivalentTo(new CookieBannerModel
        {
            ShowBanner = false,
            ShowAcknowledgement = false,
            AcceptAnalytics = true,
            ReturnUrl = $"~{PATH}{QUERY}"
        });
    }

    [TestMethod]
    public void Invoke_SetsModel_WhenOnCookiePage_And_CookieConsentStateAllFalse()
    {
        // Arrange
        var cookieConsentState = new CookieConsentState
        {
            CookieAcknowledgementRequired = false,
            CookiesAccepted = false,
            CookieExists = false
        };

        _cookieService
            .Setup(ap => ap.GetConsentState(It.IsAny<IRequestCookieCollection>(), It.IsAny<IResponseCookies>()))
            .Returns(cookieConsentState);

        var routeData = new Microsoft.AspNetCore.Routing.RouteData();
        routeData.Values.Add("controller", "Cookies");

        var component = new CookieBannerViewComponent(_cookieOptions.Object, _cookieService.Object)
        {
            ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    HttpContext = _httpContextMock.Object,
                    RouteData = routeData
                }
            }
        };

        // Act
        var result = component.Invoke() as ViewViewComponentResult;

        // Assert
        result.ViewData.Model.Should().BeEquivalentTo(new CookieBannerModel
        {
            ShowBanner = false,
            ShowAcknowledgement = false,
            AcceptAnalytics = false,
            ReturnUrl = $"~{PATH}{QUERY}"
        });
    }
}