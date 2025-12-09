namespace FrontendObligationChecker.UnitTests.Controllers;

using System.Globalization;
using FluentAssertions;
using FluentAssertions.Execution;
using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Services.PublicRegister;
using FrontendObligationChecker.ViewModels.PublicRegister;

[TestClass]
public class PublicRegisterLogicTests
{
    [TestMethod]
    public async Task TestGetRegisterViewModel()
    {
        // Arrange
        var fakeBlobStorageService = new FakeBlobStorageService([
            new FakeBlob(
                Name: "2025/Public_Register_Producers_27_November_2025.csv",
                Properties: new FakeBlobProperties(
                    ContentLength: 132629,
                    LastModified: new DateTime(2025, 9, 30, 23, 59, 59, DateTimeKind.Utc))),
            new FakeBlob(
                Name: "2026/Public_Register_Producers_27_November_2025.csv",
                Properties: new FakeBlobProperties(
                    ContentLength: 25883,
                    LastModified: new DateTime(2025, 12, 7, 23, 59, 59, DateTimeKind.Utc))),
        ]);

        // Act
        var actual = await PublicRegisterController.GetRegisterViewModel(
            isComplianceSchemesRegisterEnabled: false,
            isEnforcementActionsSectionEnabled: false,
            isPublicRegisterNextYearEnabled: true,
            optionsCurrentYear: null, // only used for manual QA
            optionsPublicRegisterPreviousYearEndMonthAndDay: "02-01",
            optionsPublicRegisterNextYearStartMonthAndDay: "11-01",
            optionsPublishedDate: new DateTime(2025, 9, 30),
            urlOptionsDefraUrl: "https://defra.example.org",
            urlOptionsBusinessAndEnvironmentUrl: "https://defra.example.org/business",
            defraHelplineEmail: "help@example.org",
            urlOptionsPublicRegisterScottishProtectionAgency: "https://defra.example.org/spa",
            getUtcNow: () => new DateTime(2025, 12, 8), // today
            blobStorageService: fakeBlobStorageService,
            optionsPublicRegisterBlobContainerName: "unused",
            optionsPublicRegisterCsoBlobContainerName: "unused");

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
                DatePublished = null,
                DateLastModified = null,
                FileName = null,
                FileSize = null,
                FileType = null,
            },
            EnforcementActionFiles = new List<EnforcementActionFileViewModel>(),
            EnglishEnforcementActionFile = null,
            WelshEnforcementActionFile = null,
            NortherIrishEnforcementActionFile = null,
            ScottishEnforcementActionFileUrl = null,
        }, options => options
            .Excluding(x => x.BackLinkToDisplay)
            .Excluding(x => x.CurrentPage)
            .Excluding(x => x.Timestamp));
    }

    [TestMethod]
    [DataRow("2025-12-08", "2025", "2026")]
    [DataRow("2026-01-01", "2025", "2026")]
    [DataRow("2026-01-31", "2025", "2026")]
    [DataRow("2026-02-01", "2025", "2026")]
    [DataRow("2026-02-02", "2026", "2027")]
    public async Task TestDateBoundaryLogic(string fakeCurrentDate, string expectedCurrentYear, string expectedNextYear)
    {
        // Arrange
        var fakeBlobStorageService = new FakeBlobStorageService([
            new FakeBlob(Name: "2025/Public_Register_Producers_27_November_2025.csv",
                Properties: new FakeBlobProperties(
                    ContentLength: 132629,
                    LastModified: new DateTime(2025, 9, 30, 23, 59, 59, DateTimeKind.Utc))),
            new FakeBlob(Name: "2026/Public_Register_Producers_27_November_2025.csv",
                Properties: new FakeBlobProperties(
                    ContentLength: 25883,
                    LastModified: new DateTime(2025, 12, 7, 23, 59, 59, DateTimeKind.Utc))),
        ]);
 
        // Act
        var guidanceViewModel = await RunGetGuidanceViewModel(fakeClock: ConvertToDateTime(fakeCurrentDate), fakeBlobStorageService: fakeBlobStorageService);

        // Assert
        using (new AssertionScope())
        {
            guidanceViewModel.Currentyear.Should().Be(expectedCurrentYear);
            guidanceViewModel.Nextyear.Should().Be(expectedNextYear);
            guidanceViewModel.LastUpdated.Should().Be("7 December 2025");
            guidanceViewModel.ProducerRegisteredFile.Should().BeEquivalentTo(new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = "2025/Public_Register_Producers_27_November_2025.csv",
                FileSize = "132629",
                FileType = "CSV",
            });
            guidanceViewModel.ProducerRegisteredFileNextYear.Should().BeEquivalentTo(new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025",
                DateLastModified = "7 December 2025",
                FileName = "2026/Public_Register_Producers_27_November_2025.csv",
                FileSize = "25883",
                FileType = "CSV",
            });
        }
    }

    private static async Task<GuidanceViewModel> RunGetGuidanceViewModel(DateTime fakeClock, FakeBlobStorageService fakeBlobStorageService)
    {
        var getRegisterViewModel = await PublicRegisterController.GetRegisterViewModel(
            isComplianceSchemesRegisterEnabled: false,
            isEnforcementActionsSectionEnabled: false,
            isPublicRegisterNextYearEnabled: true,
            optionsCurrentYear: null, // This is null in Production, looks like it's only used for testing
            optionsPublicRegisterPreviousYearEndMonthAndDay: "02-01",
            optionsPublicRegisterNextYearStartMonthAndDay: "11-01",
            optionsPublishedDate: new DateTime(2025, 9, 30, 0, 0, 0, DateTimeKind.Utc),
            urlOptionsDefraUrl: "https://defra.example.org",
            urlOptionsBusinessAndEnvironmentUrl: "https://defra.example.org/business",
            defraHelplineEmail: "help@example.org",
            urlOptionsPublicRegisterScottishProtectionAgency: "https://defra.example.org/spa",
            getUtcNow: () => fakeClock,
            blobStorageService: fakeBlobStorageService,
            optionsPublicRegisterBlobContainerName: "unused",
            optionsPublicRegisterCsoBlobContainerName: "unused");

        return getRegisterViewModel;
    }

    /// <summary>
    /// DataRow values have to be compile-time constants. Helper to convert string dates to datetime.
    /// </summary>
    private static DateTime ConvertToDateTime(string date) => DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
}

/// <summary>
/// Approximates the behaviour of <see cref="BlobStorageService"/> as of commit 645e5953854c9ab17c687296df1a57d797a45862 while avoiding the entanglement with azure blob storage apis.
/// Enables testing of GetRegisterViewModel end to end, with substituing for a "good enough" emulation of the blob handling.
/// </summary>
/// <param name="fakeBlobs">Fake blobs available in the fake storage availalbe for querying</param>
public class FakeBlobStorageService(List<FakeBlob> fakeBlobs) : IBlobStorageService
{
    public Task<PublicRegisterBlobModel> GetLatestFilePropertiesAsync(string containerName) => throw new NotImplementedException();

    public Task<Dictionary<string, PublicRegisterBlobModel>> GetLatestFilePropertiesAsync(string containerName, List<string> folderPrefixes)
    {
        // This function is a copy-paste of the code at
        // https://github.com/DEFRA/epr-obligationchecker-frontend/blob/645e5953854c9ab17c687296df1a57d797a45862/src/FrontendObligationChecker/Services/PublicRegister/BlobStorageService.cs#L61-L98
        // with minimal modifications to make it work as part of the fake
        var result = new Dictionary<string, PublicRegisterBlobModel>();
        foreach (var folderPrefix in folderPrefixes)
        {
            if (string.IsNullOrWhiteSpace(folderPrefix)) continue;

            var latestBlob = GetLatestBlobAsync(folderPrefix);
            if (latestBlob is null) continue;

            // var blobClient = containerClient.GetBlobClient(latestBlob.Name);
            // var properties = await blobClient.GetPropertiesAsync();
            var properties = latestBlob.Properties;

            var model = new PublicRegisterBlobModel
            {
                // PublishedDate = publicRegisterOptions.Value.PublishedDate,
                PublishedDate = new DateTime(1999, 01, 01, 00, 00, 00, DateTimeKind.Utc), // unused field
                Name = latestBlob.Name,
                LastModified = properties.LastModified,
                ContentLength = properties.ContentLength.ToString(),
                FileType = GetFileType(latestBlob.Name)
            };

            result[folderPrefix.TrimEnd('/')] = model;
        }

        return Task.FromResult(result);
    }

    public Task<EnforcementActionFileViewModel> GetEnforcementActionFileByAgency(string agency) => throw new NotImplementedException();

    public Task<IEnumerable<EnforcementActionFileViewModel>> GetEnforcementActionFiles() => throw new NotImplementedException();

    public Task<PublicRegisterFileModel> GetLatestFileAsync(string containerName, string blobName) => throw new NotImplementedException();

    /// <summary>
    /// Approximates behaviour of <see cref="BlobStorageService.GetLatestBlobAsync"/> as of commit 645e5953854c9ab17c687296df1a57d797a45862 without entanglement with azure apis.
    /// </summary>
    private FakeBlob? GetLatestBlobAsync(string prefix)
    {
        // This function is a copy-paste of the code at
        // https://github.com/DEFRA/epr-obligationchecker-frontend/blob/645e5953854c9ab17c687296df1a57d797a45862/src/FrontendObligationChecker/Services/PublicRegister/BlobStorageService.cs#L181-L197
        // with minimal modifications to make it work as part of the fake
        FakeBlob? latestBlob = null;

        foreach (FakeBlob blobItem in fakeBlobs.Where(b => b.Name.StartsWith(prefix)))
        {
            var contentLength = blobItem.Properties?.ContentLength ?? 0;
            var lastModified = blobItem.Properties?.LastModified;

            if (contentLength > 0 && (latestBlob == null || (lastModified != null && lastModified > latestBlob.Properties?.LastModified)))
            {
                latestBlob = blobItem;
            }
        }

        return latestBlob;
    }

    /// <summary>
    /// Exact copy of <see cref="BlobStorageService.GetFileType"/> to support test fake.
    /// </summary>
    private static string GetFileType(string blobName)
    {
        // Exacct copy of
        // https://github.com/DEFRA/epr-obligationchecker-frontend/blob/645e5953854c9ab17c687296df1a57d797a45862/src/FrontendObligationChecker/Services/PublicRegister/BlobStorageService.cs#L169-L173
        // To support test fake
        var extension = Path.GetExtension(blobName);
        return string.IsNullOrWhiteSpace(extension) ? "CSV" : extension.TrimStart('.').ToUpperInvariant();
    }
}

/// <param name="Name">This is the full path, e.g. '2025/Public_Register_Producers_27_November_2025.csv'</param>
public record FakeBlob(string Name, FakeBlobProperties Properties);
public record FakeBlobProperties(int ContentLength, DateTime LastModified);