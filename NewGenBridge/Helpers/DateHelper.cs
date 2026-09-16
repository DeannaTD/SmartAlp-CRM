using System.Globalization;

namespace NewGenBridge.Helpers
{
    public class DateHelper
    {
        public static DateTime GetStartDate(DateTime date)
        {
            date = date.Date;
            int days = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-days);
        }

        public static bool DateIsBetween(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        public static string GetMonthName(int month, bool isShort = false)
        {
            return CultureInfo.CreateSpecificCulture("en-GB").DateTimeFormat.GetMonthName(month);
        }
    }
}
