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
        // Arrange
        List<string> capturedPrefixes = null;

        // Act
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

        // Assert
        capturedPrefixes.Should().BeEquivalentTo(new[]
        {
            "2025",
            "2026",
        });

        actual.Should().BeEquivalentTo(new GuidanceViewModel
        {
            DefraUrl = "https://defra.example.org",
            BusinessAndEnvironmentUrl = "https://defra.example.org/business",
            DefraHelplineEmail = "help@example.org",
            PublishedDate = "30 September 2025",
            Currentyear = "2025",
            Nextyear = "2026",
            LastUpdated = "30 September 2025",
            ProducerRegisteredFile = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "30 September 2025",
                FileName = null,
                FileSize = "0",
                FileType = "CSV"
            },
            ProducerRegisteredFileNextYear = null,
            ComplianceSchemeRegisteredFile = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "30 September 2025",
                FileName = null,
                FileSize = "0",
                FileType = "CSV"
            },
            EnforcementActionFiles = new List<EnforcementActionFileViewModel>(),
            EnglishEnforcementActionFile = null,
            WelshEnforcementActionFile = null,
            NortherIrishEnforcementActionFile = null,
            ScottishEnforcementActionFileUrl = "https://defra.example.org/spa"
        }, options => options
            .Excluding(x => x.BackLinkToDisplay)
            .Excluding(x => x.CurrentPage)
            .Excluding(x => x.Timestamp));
    }
}