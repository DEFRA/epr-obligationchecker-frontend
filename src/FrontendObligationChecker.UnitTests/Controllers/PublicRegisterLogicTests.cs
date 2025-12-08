namespace FrontendObligationChecker.UnitTests.Controllers;

using FrontendObligationChecker.Controllers;

[TestClass]
public class PublicRegisterLogicTests
{
    [TestMethod]
    public void TestGet()
    {
        var actual = PublicRegisterController.GetRegisterViewModel(PublicRegisterController._urlOptions.PublicRegisterScottishProtectionAgency, PublicRegisterController._emailAddressOptions.DefraHelpline, PublicRegisterController._urlOptions.BusinessAndEnvironmentUrl, PublicRegisterController._urlOptions.DefraUrl, PublicRegisterController._options.PublishedDate, PublicRegisterController._options.PublicRegisterNextYearStartMonthAndDay, PublicRegisterController._options.PublicRegisterPreviousYearEndMonthAndDay, PublicRegisterController._options.CurrentYear);
        Assert.IsNotNull(actual);
    }
}