namespace FrontendObligationChecker.Services.PublicRegister;

using System.Globalization;
using FrontendObligationChecker.Models.Config;

public static class PublicRegisterCutoverHelper
{
    /// <summary>
    /// Returns the effective "now" for public-register pages, honouring the
    /// <see cref="PublicRegisterOptions.FakeDateTimeUtcNow"/> QA override when set.
    /// </summary>
    public static DateTime GetEffectiveUtcNow(PublicRegisterOptions options)
    {
        return string.IsNullOrWhiteSpace(options.FakeDateTimeUtcNow)
            ? DateTime.UtcNow
            : DateTime.ParseExact(options.FakeDateTimeUtcNow, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns true if the previous calendar year is still being shown on the main
    /// public register as of <paramref name="utcNow"/>. Strict less-than comparison
    /// against <paramref name="endMonthDay"/> ("MM-dd") so that on the cutover date
    /// itself the previous year is dropped from the main register and picked up by
    /// the archive page (no overlap between the two pages).
    /// </summary>
    public static bool IsPreviousYearStillOnMainRegister(DateTime utcNow, string? endMonthDay)
    {
        if (string.IsNullOrWhiteSpace(endMonthDay))
        {
            return false;
        }

        var currentMonthDay = utcNow.ToString("MM-dd", CultureInfo.InvariantCulture);
        return string.Compare(currentMonthDay, endMonthDay, StringComparison.Ordinal) < 0;
    }
}
