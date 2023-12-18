using FrontendObligationChecker.Services.Wrappers.Interfaces;

namespace FrontendObligationChecker.Services.Wrappers;
public class DateTimeWrapper : IDateTimeWrapper
{
    public DateTime UtcNow => DateTime.UtcNow;
}