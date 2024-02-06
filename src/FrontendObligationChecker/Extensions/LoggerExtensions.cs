namespace FrontendObligationChecker.Extensions;

using System.Runtime.CompilerServices;

public static class LoggerExtensions
{
    public static void LogEnter<T>(this ILogger<T> logger, [CallerMemberName] string methodName = "")
        => logger.LogDebug("Entering {methodName}", args: methodName);

    public static void LogExit<T>(this ILogger<T> logger, [CallerMemberName] string methodName = "")
        => logger.LogDebug("Exiting {methodName}", args: methodName);
}