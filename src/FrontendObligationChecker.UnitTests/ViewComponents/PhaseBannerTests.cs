using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.ViewComponents;
using FrontendObligationChecker.ViewModels;

using Microsoft.Extensions.Options;

namespace FrontendObligationChecker.UnitTests.ViewComponents;

[TestClass]
public class PhaseBannerTests
{
    [TestMethod]
    public void Invoke_SetsModel()
    {
        // Arrange
        var phaseBannerOptions = new PhaseBannerOptions()
        {
            ApplicationStatus = "Beta", SurveyUrl = "testUrl", Enabled = true
        };
        var options = Options.Create(phaseBannerOptions);
        var component = new PhaseBannerViewComponent(options);

        // Act
        var model = component.Invoke().ViewData.Model as PhaseBannerModel;

        // Assert
        Assert.AreEqual($"PhaseBanner.{phaseBannerOptions.ApplicationStatus}", model.Status);
        Assert.AreEqual(phaseBannerOptions.SurveyUrl, model.Url);
        Assert.AreEqual(phaseBannerOptions.Enabled, model.ShowBanner);
    }
}