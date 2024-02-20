namespace FrontendObligationChecker.UnitTests.Extensions;

using System.Runtime.InteropServices;
using Constants;
using Exceptions;
using FluentAssertions;
using FrontendObligationChecker.Extensions;
using FrontendObligationChecker.Models.Config;

[TestClass]
public class LargeProducerReportFileNamesOptionsExtensionTests
{
    [DataRow(HomeNation.England, "en.csv")]
    [DataRow("en", "en.csv")]
    [DataRow("eN", "en.csv")]
    [DataRow(HomeNation.Scotland, "sc.csv")]
    [DataRow(HomeNation.Wales, "wl.csv")]
    [DataRow(HomeNation.NorthernIreland, "ni.csv")]
    [DataRow(HomeNation.All, "all.csv")]
    [TestMethod]
    public void GetFileNameFromNationCode_ReturnsCorrectFileName(string nationCode, string fileName)
    {
        // Arrange
        var options = new LargeProducerReportFileNamesOptions
        {
            EnglishReportFileName = "en.csv",
            ScottishReportFileName = "sc.csv",
            WalesReportFileName = "wl.csv",
            NorthernIrelandReportFileName = "ni.csv",
            AllNationsReportFileName = "all.csv"
        };

        // Act
        var result = options.GetFileNameFromNationCodeAndCulture(nationCode, Language.English);

        // Assert
        result.Should().Be(fileName);
    }

    [DataRow("NA")]
    [DataRow("enz")]
    [TestMethod]
    public void GetFileNameFromNationCode_Throws_IncorrectNationCode(string nationCode)
    {
        // Arrange
        var options = new LargeProducerReportFileNamesOptions
        {
            EnglishReportFileName = "en.csv",
            ScottishReportFileName = "sc.csv",
            WalesReportFileName = "wl.csv",
            NorthernIrelandReportFileName = "ni.csv",
            AllNationsReportFileName = "all.csv"
        };

        // Act
        var act = () => options.GetFileNameFromNationCodeAndCulture(nationCode, Language.English);

        // Assert
        act.Should().Throw<HomeNationInvalidException>();
    }

    [TestMethod]
    public void GetAllNationCodeToFileNameMappings_ReturnsCorrectEnDictionary()
    {
        // Arrange
        var options = new LargeProducerReportFileNamesOptions
        {
            EnglishReportFileName = "en.csv",
            ScottishReportFileName = "sc.csv",
            WalesReportFileName = "wl.csv",
            NorthernIrelandReportFileName = "ni.csv",
            AllNationsReportFileName = "all.csv",
            EnglishReportFileNameInWelsh = "ency.csv",
            ScottishReportFileNameInWelsh = "sccy.csv",
            WalesReportFileNameInWelsh = "wlcy.csv",
            NorthernIrelandReportFileNameInWelsh = "nicy.csv",
            AllNationsReportFileNameInWelsh = "allcy.csv"
        };
        var expectedValue = new Dictionary<string, string>()
        {
            {
                HomeNation.England, "en.csv"
            },
            {
                HomeNation.Scotland, "sc.csv"
            },
            {
                HomeNation.Wales, "wl.csv"
            },
            {
                HomeNation.NorthernIreland, "ni.csv"
            },
            {
                HomeNation.All, "all.csv"
            },
        };

        // Act
        var result = options.GetAllNationCodeToFileNameMappings(Language.English);

        // Assert
        result.Should().BeEquivalentTo(expectedValue);
    }

    [TestMethod]
    public void GetAllNationCodeToFileNameMappings_ReturnsCorrectWelshDictionary()
    {
        // Arrange
        var options = new LargeProducerReportFileNamesOptions
        {
            EnglishReportFileName = "en.csv",
            ScottishReportFileName = "sc.csv",
            WalesReportFileName = "wl.csv",
            NorthernIrelandReportFileName = "ni.csv",
            AllNationsReportFileName = "all.csv",
            EnglishReportFileNameInWelsh = "ency.csv",
            ScottishReportFileNameInWelsh = "sccy.csv",
            WalesReportFileNameInWelsh = "wlcy.csv",
            NorthernIrelandReportFileNameInWelsh = "nicy.csv",
            AllNationsReportFileNameInWelsh = "allcy.csv"
        };
        var expectedValue = new Dictionary<string, string>()
        {
            {
                HomeNation.England, "ency.csv"
            },
            {
                HomeNation.Scotland, "sccy.csv"
            },
            {
                HomeNation.Wales, "wlcy.csv"
            },
            {
                HomeNation.NorthernIreland, "nicy.csv"
            },
            {
                HomeNation.All, "allcy.csv"
            },
        };

        // Act
        var result = options.GetAllNationCodeToFileNameMappings(Language.Welsh);

        // Assert
        result.Should().BeEquivalentTo(expectedValue);
    }
}