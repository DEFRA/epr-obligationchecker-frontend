namespace FrontendObligationChecker.Extensions;

using Constants;
using Exceptions;
using Models.Config;

public static class LargeProducerReportFileNamesOptionsExtension
{
    private const string HomeNationIsInvalid = "Home nation is invalid";

    public static string GetFileNameFromNationCode(this LargeProducerReportFileNamesOptions options, string nationCode)
    {
        string fileName = nationCode.ToUpper() switch
        {
            HomeNation.England => options.En,
            HomeNation.Scotland => options.Sc,
            HomeNation.Wales => options.Wl,
            HomeNation.NorthernIreland => options.Ni,
            HomeNation.All => options.All,
            _ => throw new HomeNationInvalidException(HomeNationIsInvalid)
        };
        return fileName;
    }
}