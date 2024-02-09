namespace FrontendObligationChecker.Helpers;

using ByteSizeLib;

public static class FileSizeFormatterHelper
{
    public static string ConvertByteSizeToString(ByteSize bytes)
    {
        return bytes.KiloBytes switch
        {
            < 0.5 => $"{bytes.Bytes:F0}B",
            <= 999 => $"{Math.Ceiling(bytes.KiloBytes):F0}KB",
            _ => $"{Math.Ceiling(bytes.MegaBytes * 10) / 10:F1}MB"
        };
    }
}