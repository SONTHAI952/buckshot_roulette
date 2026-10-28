using System;
using System.Globalization;

namespace ZeroX.Extensions
{
    public static class DateTimeExtension
    {
        public static bool IsToday(string dateTimeString)
        {
            DateTime now = DateTime.Now.Date;
            DateTime date = ZFormat.StringToDateTime(dateTimeString, ZFormat.DateTimeFormat.OnlyDate);
            return (int) (now - date).TotalDays == 0;
        }

        public static bool IsToday(DateTime date)
        {
            DateTime now = DateTime.Now.Date;
            return (int) (now - date.Date).TotalDays == 0;
        }

        public static int GetWeekOfYear(this DateTime dateTime)
        {
            CultureInfo myCI = CultureInfo.CurrentCulture;
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            return myCal.GetWeekOfYear(dateTime, myCWR, myFirstDOW);
        }

        public static DateTime GetStartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }
    }
}