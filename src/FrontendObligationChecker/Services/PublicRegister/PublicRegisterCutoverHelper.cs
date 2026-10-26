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
    /// Returns true if the register for <paramref name="year"/> has been moved from the main
    /// public register to the archive page, i.e. it is on or before
    /// <see cref="PublicRegisterOptions.ArchivedUpToYear"/>. The current year is never archived,
    /// and a null <paramref name="archivedUpToYear"/> archives nothing. Both pages use this so the
    /// same year is never displayed on both at once.
    /// </summary>
    /// <param name="year">The register year to check, e.g. 2025.</param>
    /// <param name="currentYear">The current calendar year.</param>
    /// <param name="archivedUpToYear">The configured <see cref="PublicRegisterOptions.ArchivedUpToYear"/>.</param>
    /// <returns>True if the year belongs on the archive page.</returns>
    public static bool IsYearArchived(int year, int currentYear, int? archivedUpToYear)
    {
        return archivedUpToYear.HasValue
            && year <= archivedUpToYear.Value
            && year < currentYear;
    }
}
