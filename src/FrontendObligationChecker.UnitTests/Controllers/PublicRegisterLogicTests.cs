namespace FrontendObligationChecker.UnitTests.Controllers;

using System.Globalization;
using FluentAssertions;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.ViewModels.PublicRegister;

[TestClass]
public class PublicRegisterLogicTests
{
    [TestMethod]
    public async Task TestGetRegisterViewModel()
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
            getUtcNow: () => new DateTime(2025, 12, 8), // today
            getLatestFilePropertiesForPrefixes: folderPrefixes =>
            {
                capturedPrefixes = folderPrefixes;
                return Task.FromResult(new Dictionary<string, PublicRegisterBlobModel>
                {
                    {
                        "2025", new PublicRegisterBlobModel
                        {
                            ContentLength = "132629",
                            EnforcementActionItems = null,
                            FileType = "CSV",
                            LastModified = new DateTime(2025, 12, 7, 19, 59, 58), // yesterday
                            Name = "2025/Public_Register_Producers_27_November_2025.csv",
                            PublishedDate = new DateTime(2025, 11, 3, 23, 57, 56), // published previously (as seen in dev storage)
                        }
                    },
                    {
                        "2026", new PublicRegisterBlobModel
                        {
                            ContentLength = "25883",
                            EnforcementActionItems = null,
                            FileType = "CSV",
                            LastModified = new DateTime(2025, 12, 7, 19, 45, 46), // yesterday
                            Name = "2026/Public_Register_Producers_27_November_2025.csv",
                            PublishedDate = new DateTime(2025, 11, 3, 23, 59, 59), // published previously (as seen in dev storage)
                        }
                    },
                });
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
            LastUpdated = "7 December 2025",
            ProducerRegisteredFile = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = "2025/Public_Register_Producers_27_November_2025.csv",
                FileSize = "132629",
                FileType = "CSV",
            },
            ProducerRegisteredFileNextYear = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = "2026/Public_Register_Producers_27_November_2025.csv",
                FileSize = "25883",
                FileType = "CSV",
            },
            ComplianceSchemeRegisteredFile = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = null,
                FileSize = "0",
                FileType = "CSV",
            },
            EnforcementActionFiles = new List<EnforcementActionFileViewModel>(),
            EnglishEnforcementActionFile = null,
            WelshEnforcementActionFile = null,
            NortherIrishEnforcementActionFile = null,
            ScottishEnforcementActionFileUrl = "https://defra.example.org/spa",
        }, options => options
            .Excluding(x => x.BackLinkToDisplay)
            .Excluding(x => x.CurrentPage)
            .Excluding(x => x.Timestamp));
    }

    [TestMethod]
    [DataRow("2025-12-08", "2025", "2026", new[] { "2025", "2026" })]
    [DataRow("2026-01-01", "2025", "2026", new[] { "2025", "2026" })]
    [DataRow("2026-01-31", "2025", "2026", new[] { "2025", "2026" })]
    [DataRow("2026-02-01", "2025", "2026", new[] { "2025", "2026" })]
    [DataRow("2026-02-02", "2026", "2027", new[] { "2026" })]
    public async Task TestDateBoundaryLogic(string now, string expectedCurrentYear, string expectedNextYear, string[] expectedPrefixes)
    {
        // Arrange
        var dateTime = ConvertToDateTime(now);

        // Act
        var guidanceViewModelResult = await GetGuidanceViewModel(dateTime);

        // Assert
        guidanceViewModelResult.FolderPrefixes.Should().BeEquivalentTo(expectedPrefixes);

        guidanceViewModelResult.ViewModel.Should().BeEquivalentTo(new GuidanceViewModel
        {
            DefraUrl = "https://defra.example.org",
            BusinessAndEnvironmentUrl = "https://defra.example.org/business",
            DefraHelplineEmail = "help@example.org",
            PublishedDate = "30 September 2025",
            Currentyear = expectedCurrentYear,
            Nextyear = expectedNextYear,
            LastUpdated = "7 December 2025",
            ProducerRegisteredFile = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = "2025/Public_Register_Producers_27_November_2025.csv",
                FileSize = "132629",
                FileType = "CSV",
            },
            ProducerRegisteredFileNextYear = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = "2026/Public_Register_Producers_27_November_2025.csv",
                FileSize = "25883",
                FileType = "CSV",
            },
            ComplianceSchemeRegisteredFile = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = null,
                FileSize = "0",
                FileType = "CSV",
            },
            EnforcementActionFiles = new List<EnforcementActionFileViewModel>(),
            EnglishEnforcementActionFile = null,
            WelshEnforcementActionFile = null,
            NortherIrishEnforcementActionFile = null,
            ScottishEnforcementActionFileUrl = "https://defra.example.org/spa",
        }, options => options
            .Excluding(x => x.BackLinkToDisplay)
            .Excluding(x => x.CurrentPage)
            .Excluding(x => x.Timestamp));
    }

    private static DateTime ConvertToDateTime(string date)
    {
        return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static async Task<GuidanceViewModelResult> GetGuidanceViewModel(DateTime utcNow)
    {
        var capturedPrefixes = new List<string>();

        var getRegisterViewModel = await PublicRegisterController.GetRegisterViewModel(
            isComplianceSchemesRegisterEnabled: true,
            isEnforcementActionsSectionEnabled: true,
            isPublicRegisterNextYearEnabled: true,
            optionsCurrentYear: null,                   // This is null in Production, looks like it's only used for testing
            optionsPublicRegisterPreviousYearEndMonthAndDay: "02-01",
            optionsPublicRegisterNextYearStartMonthAndDay: "11-01",
            optionsPublishedDate: new DateTime(2025, 9, 30, 0, 0, 0, DateTimeKind.Utc),
            urlOptionsDefraUrl: "https://defra.example.org",
            urlOptionsBusinessAndEnvironmentUrl: "https://defra.example.org/business",
            defraHelplineEmail: "help@example.org",
            urlOptionsPublicRegisterScottishProtectionAgency: "https://defra.example.org/spa",
            getUtcNow: () => utcNow,
            getLatestFilePropertiesForPrefixes: folderPrefixes =>
            {
                capturedPrefixes = folderPrefixes;
                return Task.FromResult(new Dictionary<string, PublicRegisterBlobModel>
                {
                    {
                        "2025", new PublicRegisterBlobModel
                        {
                            ContentLength = "132629",
                            EnforcementActionItems = null,
                            FileType = "CSV",
                            // yesterday
                            LastModified = new DateTime(2025, 12, 7, 19, 59, 58, DateTimeKind.Utc),
                            Name = "2025/Public_Register_Producers_27_November_2025.csv",
                            // published previously (as seen in dev storage)
                            PublishedDate = new DateTime(2025, 11, 3, 23, 57, 56, DateTimeKind.Utc)
                        }
                    },
                    {
                        "2026", new PublicRegisterBlobModel
                        {
                            ContentLength = "25883",
                            EnforcementActionItems = null,
                            FileType = "CSV",
                            // yesterday
                            LastModified = new DateTime(2025, 12, 7, 19, 45, 46, DateTimeKind.Utc),
                            Name = "2026/Public_Register_Producers_27_November_2025.csv",
                            // published previously (as seen in dev storage)
                            PublishedDate = new DateTime(2025, 11, 3, 23, 59, 59, DateTimeKind.Utc)
                        }
                    },
                });
            },
            getComplianceSchemeFileProperties: () => Task.FromResult(new PublicRegisterBlobModel()),
            getEnforcementActionFiles: () => Task.FromResult(new List<EnforcementActionFileViewModel>().AsEnumerable()));

        return new GuidanceViewModelResult(capturedPrefixes, getRegisterViewModel);
    }
}

public record GuidanceViewModelResult(List<string> FolderPrefixes, GuidanceViewModel ViewModel);