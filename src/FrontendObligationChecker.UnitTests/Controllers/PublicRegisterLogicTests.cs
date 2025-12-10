namespace FrontendObligationChecker.UnitTests.Controllers;

using System.Diagnostics;
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
            PublishedDate = "30 September 2025", // shown in footer, controlled via config only
            Currentyear = "2025", // shown in download link as "2027 Public register of producers"
            Nextyear = "2026", // shown in download link as "2027 Public register of producers"
            LastUpdated = "7 December 2025", // shown in footer - is the newest file's modified date
            ProducerRegisteredFile = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025", // unused in UI
                DateLastModified = "7 December 2025", // unused in UI
                FileName = "2025/Public_Register_Producers_27_November_2025.csv",
                FileSize = "132629",
                FileType = "CSV",
            },
            ProducerRegisteredFileNextYear = new PublicRegisterFileViewModel
            {
                DatePublished = "30 September 2025", // unused in UI
                DateLastModified = "7 December 2025", // unused in UI
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
    // Characterization tests - capture current behaviour of logic.
    [DataRow(false, "2025-12-09", "2025", "2026", "8 December 2025", "2025/Public_Register_Producers_08_December_2025.csv", "20250354", null, null)] // now
    [DataRow(false, "2025-12-31", "2025", "2026", "30 December 2025", "2025/Public_Register_Producers_30_December_2025.csv", "20250376", null, null)] // end of year
    [DataRow(false, "2026-01-01", "2025", "2026", "31 December 2025", "2025/Public_Register_Producers_31_December_2025.csv", "20250377", "2026/Public_Register_Producers_31_December_2025.csv", "20260377")] // new-year's day
    [DataRow(false, "2026-01-02", "2025", "2026", "1 January 2026", "2025/Public_Register_Producers_01_January_2026.csv", "20250378", "2026/Public_Register_Producers_01_January_2026.csv", "20260378")] // 2nd Jan
    [DataRow(false, "2026-01-31", "2025", "2026", "30 January 2026", "2025/Public_Register_Producers_30_January_2026.csv", "20250407", "2026/Public_Register_Producers_30_January_2026.csv", "20260407")]
    [DataRow(false, "2026-02-01", "2025", "2026", "31 January 2026", "2025/Public_Register_Producers_31_January_2026.csv", "20250408", "2026/Public_Register_Producers_31_January_2026.csv", "20260408")] // Configured date for PublicRegister__PublicRegisterPreviousYearEndMonthAndDay "02-01"
    [DataRow(false, "2026-02-02", "2026", "2027", "1 February 2026", "2026/Public_Register_Producers_01_February_2026.csv", "20260409", null, null)] // after next year's PublicRegister__PublicRegisterNextYearStartMonthAndDay
    [DataRow(true, "2025-12-09", "2025", "2026", "8 December 2025", "2025/Public_Register_Producers_08_December_2025.csv", "20250354", "2026/Public_Register_Producers_08_December_2025.csv", "20260354")] // now
    [DataRow(true, "2025-12-31", "2025", "2026", "30 December 2025", "2025/Public_Register_Producers_30_December_2025.csv", "20250376", "2026/Public_Register_Producers_30_December_2025.csv", "20260376")] // end of year
    [DataRow(true, "2026-01-01", "2025", "2026", "31 December 2025", "2025/Public_Register_Producers_31_December_2025.csv", "20250377", "2026/Public_Register_Producers_31_December_2025.csv", "20260377")] // new-year's day
    [DataRow(true, "2026-01-02", "2025", "2026", "1 January 2026", "2025/Public_Register_Producers_01_January_2026.csv", "20250378", "2026/Public_Register_Producers_01_January_2026.csv", "20260378")] // 2nd Jan
    [DataRow(true, "2026-01-31", "2025", "2026", "30 January 2026", "2025/Public_Register_Producers_30_January_2026.csv", "20250407", "2026/Public_Register_Producers_30_January_2026.csv", "20260407")]
    [DataRow(true, "2026-02-01", "2025", "2026", "31 January 2026", "2025/Public_Register_Producers_31_January_2026.csv", "20250408", "2026/Public_Register_Producers_31_January_2026.csv", "20260408")] // Configured date for PublicRegister__PublicRegisterPreviousYearEndMonthAndDay "02-01"
    [DataRow(true, "2026-02-02", "2026", "2027", "1 February 2026", "2026/Public_Register_Producers_01_February_2026.csv", "20260409", null, null)] // after next year's PublicRegister__PublicRegisterNextYearStartMonthAndDay
    public async Task TestDateBoundaryLogic(bool publicRegisterNextYearEnabled, string fakeCurrentDate, string expectedCurrentFileDisplayYear, string expectedNextFileDisplayYear, string expectedPageLastUpdated,
        string expectedFile1Filename, string expectedFile1Size,
        string? expectedFile2Filename, string expectedFile2Size)
    {
        var fakeCurrentDateTime = ConvertToDateTime(fakeCurrentDate); // boilerplate, can't use datetimes directly in DataRow as they aren't compile-time constants

        // Arrange
        // Add fake blob CSVs up to "yesterday"
        var registerCsvBlobList = BuildRealisticPublicRegisterCsvBlobList(untilDate: fakeCurrentDateTime.AddDays(-1));
        var fakeBlobStorageService = new FakeBlobStorageService(registerCsvBlobList);

        // Act
        // run the logic under test
        var guidanceViewModel = await RunGetGuidanceViewModel(fakeCurrentDateTime, fakeBlobStorageService, publicRegisterNextYearEnabled);

        // Assert
        using (new AssertionScope())
        {
            guidanceViewModel.Currentyear.Should().Be(expectedCurrentFileDisplayYear);
            guidanceViewModel.Nextyear.Should().Be(expectedNextFileDisplayYear);
            guidanceViewModel.LastUpdated.Should().Be(expectedPageLastUpdated);
            guidanceViewModel.ProducerRegisteredFile.FileName.Should().Be(expectedFile1Filename);
            guidanceViewModel.ProducerRegisteredFile.FileSize.Should().Be(expectedFile1Size);
            if (expectedFile2Filename == null)
            {
                guidanceViewModel.ProducerRegisteredFileNextYear.Should().BeNull();
            }
            else
            {
                guidanceViewModel.ProducerRegisteredFileNextYear.Should().NotBeNull();
                guidanceViewModel.ProducerRegisteredFileNextYear.FileName.Should().Be(expectedFile2Filename);
                guidanceViewModel.ProducerRegisteredFileNextYear.FileSize.Should().Be(expectedFile2Size);
            }
        }
    }

    /// <summary>
    /// Build a list of fake file entries for the fake blob storage.
    /// Logic:
    /// - Companies can register for next year, so there registrations available in a future year folder
    /// - When the synapse pipeline is run it creates a file for *every* year that has regsitrations available in the range 2025-2034
    /// - file paths are: yyyy/Public_Register_Producers_dd_MMMM_yyyy.csv where:
    ///     - the prefix `yyyy/` is the year of the collected registrations within that file
    ///       (which can be for the NEXT year because companies can register in advance for the following year), and...
    ///     - the `dd_MMMM_yyyy` is when the csv file was generated
    /// </summary>
    private static List<FakeBlob> BuildRealisticPublicRegisterCsvBlobList(DateTime untilDate)
    {
        var fakeBlobs = new List<FakeBlob>();
        var startDate = new DateTime(2024, 12, 20); // Start in 2024 to simulate existence of older folder(s)
        var blobDate = startDate;
        var fakeIncrementalContentLength = 1; // abuse the content length to give interesting unique ids to each file. In reality they would vary randomly but we need them deterministic for the tests.
        while (blobDate <= untilDate)
        {
            fakeBlobs.AddRange(
                SimulateSynapseRun(blobDate, fakeIncrementalContentLength));

            blobDate = blobDate.AddDays(1);
            fakeIncrementalContentLength++;
        }

        return fakeBlobs;
    }

    private static List<FakeBlob> SimulateSynapseRun(DateTime pipelineRunDate, int incrementalContentLength)
    {
        var fakeBlobs = new List<FakeBlob>();
        var registrationYearsWithData = new[] { 2024, 2025, 2026 };
        // simulate a single run of the synapse pipeline
        foreach (var registrationYear in registrationYearsWithData)
        {
            var hackyContentLengthWithYearAndIncrement = (registrationYear * 10000) + incrementalContentLength;
            fakeBlobs.Add(FakeRegisterCsv(registrationYear, pipelineRunDate, hackyContentLengthWithYearAndIncrement));
        }

        return fakeBlobs;
    }

    private static FakeBlob FakeRegisterCsv(int registrationYear, DateTime dateGenerated, int contentLength)
    {
        DateTime lastModified = dateGenerated.AddHours(19).AddMinutes(55).AddSeconds(56); // scheduled to generate daily ~ 7pm
        return new FakeBlob(Name: $"{registrationYear}/Public_Register_Producers_{dateGenerated:dd_MMMM_yyyy}.csv",
            Properties: new FakeBlobProperties(
                ContentLength: contentLength,
                LastModified: lastModified));
    }

    private static async Task<GuidanceViewModel> RunGetGuidanceViewModel(DateTime fakeCurrentDateTime, FakeBlobStorageService fakeBlobStorageService, bool publicRegisterNextYearEnabled)
    {
        var getRegisterViewModel = await PublicRegisterController.GetRegisterViewModel(
            isComplianceSchemesRegisterEnabled: false,
            isEnforcementActionsSectionEnabled: false,
            isPublicRegisterNextYearEnabled: publicRegisterNextYearEnabled,
            optionsCurrentYear: null, // This is null in Production, looks like it's only used for testing
            optionsPublicRegisterPreviousYearEndMonthAndDay: "02-01",
            optionsPublicRegisterNextYearStartMonthAndDay: "11-01",
            optionsPublishedDate: new DateTime(2025, 9, 30, 0, 0, 0, DateTimeKind.Utc),
            urlOptionsDefraUrl: "https://defra.example.org",
            urlOptionsBusinessAndEnvironmentUrl: "https://defra.example.org/business",
            defraHelplineEmail: "help@example.org",
            urlOptionsPublicRegisterScottishProtectionAgency: "https://defra.example.org/spa",
            getUtcNow: () => fakeCurrentDateTime,
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
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record FakeBlob(string Name, FakeBlobProperties Properties)
{
    public string DebuggerDisplay => $"{Name}, {Properties.ContentLength}, {Properties.LastModified}";
}

public record FakeBlobProperties(int ContentLength, DateTime LastModified);