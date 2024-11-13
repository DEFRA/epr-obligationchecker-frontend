namespace FrontendObligationChecker.IntegrationTests.Extensions;

public static class ApostropheToHexHelper
{
    public static string ApostropheToHex(this string text)
    {
        return text.Replace("'", "&#x27;");
    }

    public static string CurvedApostropheToHex(this string text)
    {
        return text.Replace("’", "&#x2019;");
    }
}