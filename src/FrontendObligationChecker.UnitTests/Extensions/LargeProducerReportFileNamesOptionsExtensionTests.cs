namespace FrontendObligationChecker.UnitTests.Extensions;

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
            En = "en.csv",
            Sc = "sc.csv",
            Wl = "wl.csv",
            Ni = "ni.csv",
            All = "all.csv"
        };

        // Act
        var result = options.GetFileNameFromNationCode(nationCode);

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
            En = "en.csv",
            Sc = "sc.csv",
            Wl = "wl.csv",
            Ni = "ni.csv",
            All = "all.csv"
        };

        // Act
        var act = () => options.GetFileNameFromNationCode(nationCode);

        // Assert
        act.Should().Throw<HomeNationInvalidException>();
    }
}