namespace FrontendObligationChecker.UnitTests.Helpers;

using ByteSizeLib;
using FluentAssertions;
using FrontendObligationChecker.Helpers;

[TestClass]
public class FileSizeFormatterHelperTests
{
    [TestMethod]
    [DataRow(400, "400B")]
    [DataRow(501, "1KB")]
    [DataRow(1000, "1KB")]
    [DataRow(1001, "2KB")]
    [DataRow(1500, "2KB")]
    [DataRow(900000, "900KB")]
    [DataRow(999000, "999KB")]
    [DataRow(999001, "1.0MB")]
    [DataRow(1000001, "1.1MB")]
    [DataRow(1900001, "2.0MB")]
    public void ConvertByteSizeToString_ReturnsCorrectValues(long bytes, string expectedFormattedSize)
    {
        // Arrange
        var byteSize = ByteSize.FromBytes(bytes);

        // Act
        var result = FileSizeFormatterHelper.ConvertByteSizeToString(byteSize);

        // Assert
        result.Should().Be(expectedFormattedSize);
    }
}