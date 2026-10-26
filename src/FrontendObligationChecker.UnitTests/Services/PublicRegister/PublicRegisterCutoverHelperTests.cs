namespace FrontendObligationChecker.UnitTests.Services.PublicRegister;

using System.Globalization;
using FluentAssertions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Services.PublicRegister;

[TestClass]
public class PublicRegisterCutoverHelperTests
{
    [TestMethod]
    // ArchivedUpToYear 2026 with registers for 2025, 2026 and 2027
    [DataRow(2025, 2027, 2026, true)] // before the setting — archived
    [DataRow(2026, 2027, 2026, true)] // the setting year itself — archived
    [DataRow(2027, 2027, 2026, false)] // after the setting — stays on main register
    // ArchivedUpToYear 2025 (live on release)
    [DataRow(2025, 2026, 2025, true)]
    [DataRow(2026, 2026, 2025, false)]
    [DataRow(2026, 2027, 2025, false)] // previous year stays on main register until the setting is updated
    // The current year is never archived, even if the setting covers it
    [DataRow(2027, 2027, 2027, false)]
    [DataRow(2027, 2027, 2028, false)]
    public void IsYearArchived_ReturnsExpected(int year, int currentYear, int archivedUpToYear, bool expected)
    {
        var actual = PublicRegisterCutoverHelper.IsYearArchived(year, currentYear, archivedUpToYear);

        actual.Should().Be(expected);
    }

    [TestMethod]
    [DataRow(2024)]
    [DataRow(2025)]
    [DataRow(2026)]
    public void IsYearArchived_ReturnsFalse_WhenArchivedUpToYearNotSet(int year)
    {
        var actual = PublicRegisterCutoverHelper.IsYearArchived(year, 2027, null);

        actual.Should().BeFalse();
    }

    [TestMethod]
    public void GetEffectiveUtcNow_ReturnsRealUtcNow_WhenOverrideNotSet()
    {
        var options = new PublicRegisterOptions { FakeDateTimeUtcNow = null };

        var before = DateTime.UtcNow;
        var actual = PublicRegisterCutoverHelper.GetEffectiveUtcNow(options);
        var after = DateTime.UtcNow;

        actual.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [TestMethod]
    [DataRow("2027-01-10")]
    [DataRow("2028-02-29")]
    public void GetEffectiveUtcNow_ReturnsParsedOverride_WhenSet(string fakeDate)
    {
        var options = new PublicRegisterOptions { FakeDateTimeUtcNow = fakeDate };

        var actual = PublicRegisterCutoverHelper.GetEffectiveUtcNow(options);

        actual.Should().Be(DateTime.ParseExact(fakeDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
