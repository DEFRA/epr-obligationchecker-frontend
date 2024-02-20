namespace FrontendObligationChecker.Extensions;

using Constants;
using Exceptions;
using Models.Config;

public static class LargeProducerReportFileNamesOptionsExtension
{
    private const string HomeNationIsInvalid = "Home nation is invalid";

    public static string GetFileNameFromNationCodeAndCulture(this LargeProducerReportFileNamesOptions options, string nationCode, string culture)
    {
        if (culture == Language.English)
        {
            return nationCode.ToUpper() switch
            {
                HomeNation.England => options.EnglishReportFileName,
                HomeNation.Scotland => options.ScottishReportFileName,
                HomeNation.Wales => options.WalesReportFileName,
                HomeNation.NorthernIreland => options.NorthernIrelandReportFileName,
                HomeNation.All => options.AllNationsReportFileName,
                _ => throw new HomeNationInvalidException(HomeNationIsInvalid)
            };
        }

        return nationCode.ToUpper() switch
        {
            HomeNation.England => options.EnglishReportFileNameInWelsh,
            HomeNation.Scotland => options.ScottishReportFileNameInWelsh,
            HomeNation.Wales => options.WalesReportFileNameInWelsh,
            HomeNation.NorthernIreland => options.NorthernIrelandReportFileNameInWelsh,
            HomeNation.All => options.AllNationsReportFileNameInWelsh,
            _ => throw new HomeNationInvalidException(HomeNationIsInvalid)
        };
    }

    public static Dictionary<string, string> GetAllNationCodeToFileNameMappings(this LargeProducerReportFileNamesOptions options, string culture)
    {
        return new Dictionary<string, string>
        {
            {
                HomeNation.England, GetFileNameFromNationCodeAndCulture(options, HomeNation.England, culture)
            },
            {
                HomeNation.Scotland, GetFileNameFromNationCodeAndCulture(options, HomeNation.Scotland, culture)
            },
            {
                HomeNation.Wales, GetFileNameFromNationCodeAndCulture(options, HomeNation.Wales, culture)
            },
            {
                HomeNation.NorthernIreland, GetFileNameFromNationCodeAndCulture(options, HomeNation.NorthernIreland, culture)
            },
            {
                HomeNation.All, GetFileNameFromNationCodeAndCulture(options, HomeNation.All, culture)
            }
        };
    }
}