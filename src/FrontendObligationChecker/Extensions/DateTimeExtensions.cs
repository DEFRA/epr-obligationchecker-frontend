﻿namespace FrontendObligationChecker.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime UtcToGmt(this DateTime dateTime) => TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));
    }
}