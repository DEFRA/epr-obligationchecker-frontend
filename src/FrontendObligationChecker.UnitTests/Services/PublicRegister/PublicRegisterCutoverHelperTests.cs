namespace FrontendObligationChecker.UnitTests.Services.PublicRegister;

using System.Globalization;
using FluentAssertions;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Services.PublicRegister;

[TestClass]
public class PublicRegisterCutoverHelperTests
{
    [TestMethod]
    // Cutover string "02-01" — strict less-than semantics
    [DataRow("2027-01-01", "02-01", true)] // start of year — previous year still on main
    [DataRow("2027-01-10", "02-01", true)]
    [DataRow("2027-01-31", "02-01", true)] // last day previous year is shown on main
    [DataRow("2027-02-01", "02-01", false)] // cutover day — main drops, archive picks up
    [DataRow("2027-02-02", "02-01", false)]
    [DataRow("2027-03-01", "02-01", false)]
    [DataRow("2027-12-31", "02-01", false)]
    // Leap year — cutover unaffected by Feb 29
    [DataRow("2028-01-31", "02-01", true)]
    [DataRow("2028-02-01", "02-01", false)]
    [DataRow("2028-02-29", "02-01", false)]
    public void IsPreviousYearStillOnMainRegister_ReturnsExpected(string utcNowString, string endMonthDay, bool expected)
    {
        var utcNow = DateTime.ParseExact(utcNowString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var actual = PublicRegisterCutoverHelper.IsPreviousYearStillOnMainRegister(utcNow, endMonthDay);

        actual.Should().Be(expected);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void IsPreviousYearStillOnMainRegister_ReturnsFalse_WhenEndMonthDayMissing(string? endMonthDay)
    {
        var actual = PublicRegisterCutoverHelper.IsPreviousYearStillOnMainRegister(
            new DateTime(2027, 1, 10), endMonthDay);

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
