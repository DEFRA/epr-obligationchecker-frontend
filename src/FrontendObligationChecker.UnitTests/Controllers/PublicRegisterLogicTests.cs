namespace FrontendObligationChecker.UnitTests.Controllers;

using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.ViewModels.PublicRegister;

[TestClass]
public class PublicRegisterLogicTests
{
    [TestMethod]
    public async Task TestGet()
    {
        List<string> capturedPrefixes = null;
        var actual = await PublicRegisterController.GetRegisterViewModel(
            isComplianceSchemesRegisterEnabled: true,
            isEnforcementActionsSectionEnabled: true,
            isPublicRegisterNextYearEnabled: true,
            optionsCurrentYear: "2025",
            optionsPublicRegisterPreviousYearEndMonthAndDay: "02-01",
            optionsPublicRegisterNextYearStartMonthAndDay: "11-01",
            optionsPublishedDate: new DateTime(2025, 9, 30),
            urlOptionsDefraUrl: "https://defra.example.org",
            urlOptionsBusinessAndEnvironmentUrl: "https://defra.example.org/business",
            defraHelplineEmail: "help@example.org",
            urlOptionsPublicRegisterScottishProtectionAgency: "https://defra.example.org/spa",
            getUtcNow: () => new DateTime(2025, 12, 8),
            getLatestFilePropertiesForPrefixes: (folderPrefixes) =>
            {
                capturedPrefixes = folderPrefixes;
                return Task.FromResult(new Dictionary<string, PublicRegisterBlobModel>());
            },
            getComplianceSchemeFileProperties: () => Task.FromResult(new PublicRegisterBlobModel()),
            getEnforcementActionFiles: () => Task.FromResult(new List<EnforcementActionFileViewModel>().AsEnumerable()));

        Assert.IsNotNull(actual);

        capturedPrefixes.Should().BeEquivalentTo(new[]
        {
            "2025",
            "2026",
        });
    }
}