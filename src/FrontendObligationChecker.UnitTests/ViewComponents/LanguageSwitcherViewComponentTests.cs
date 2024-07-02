namespace FrontendObligationChecker.UnitTests.ViewComponents;

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using FrontendObligationChecker.Constants;
using FrontendObligationChecker.ViewComponents;
using FrontendObligationChecker.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Moq;

[TestClass]
public class LanguageSwitcherViewComponentTests
{
    [TestMethod]
    public async Task Invoke_RendersCorrectView()
    {
        // Arrange
        const string PATH = "/test";
        const string QUERY = "?test=true";
        const string CURRENT_CULTURE = Language.English;

        var options = new RequestLocalizationOptions();
        options.AddSupportedCultures(Language.English, Language.Welsh);

        var systemUnderTest = new LanguageSwitcherViewComponent(Options.Create(options));

        var httpContext = new Mock<HttpContext>();
        var httpRequest = new Mock<HttpRequest>();
        httpRequest.Setup(x => x.Path).Returns(PATH);
        httpRequest.Setup(x => x.QueryString).Returns(new QueryString(QUERY));
        httpContext.Setup(x => x.Features.Get<IRequestCultureFeature>())
            .Returns(new RequestCultureFeature(new RequestCulture(CURRENT_CULTURE), null));
        httpContext.Setup(x => x.Request).Returns(httpRequest.Object);
        systemUnderTest.ViewComponentContext = new ViewComponentContext
        {
            ViewContext = new ViewContext { HttpContext = httpContext.Object }
        };

        // Act
        var result = systemUnderTest.Invoke() as ViewViewComponentResult;

        // Assert
        result.ViewData.Model.Should().BeEquivalentTo(new LanguageSwitcherModel
        {
            SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo(Language.English),
                    new CultureInfo(Language.Welsh)
                },
            CurrentCulture = new CultureInfo(CURRENT_CULTURE),
            ReturnUrl = $"~{PATH}"
        });
    }
}