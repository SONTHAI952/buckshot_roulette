using System;
using System.Globalization;
using UnityEngine;

namespace ZeroX
{
    public static class ZFormat
    {
        public enum DateTimeFormat
        {
            OnlyDate,
            OnlyTime,
            DateAndTime
        }



        public static string Format(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        #region Date Time

        public static string FormatTime(string format, long time)
        {
            switch (format)
            {
                case "mm:ss":
                {
                    long minutes = time / 60;
                    long seconds = time % 60;
                    return string.Format("{0:00}:{1:00}", minutes, seconds);
                }
                case "hh:mm:ss":
                {
                    long hours = time / 3600;
                    long minutes = (time - hours * 3600) / 60;
                    long seconds = time % 60;
                    return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                }
                default:
                    throw new Exception("Format không hợp lệ hoặc chưa code");
            }
        }

        public static string DateTimeToString(DateTime dateTime, DateTimeFormat dateTimeFormat)
        {
            switch (dateTimeFormat)
            {
                case DateTimeFormat.OnlyDate:
                {
                    return dateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                case DateTimeFormat.OnlyTime:
                {
                    return dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                }
                case DateTimeFormat.DateAndTime:
                {
                    return dateTime.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                default:
                    throw new Exception("Chưa code định dạng này");
            }
        }

        public static DateTime StringToDateTime(string timeString, DateTimeFormat dateTimeFormat)
        {
            switch (dateTimeFormat)
            {
                case DateTimeFormat.OnlyDate:
                {
                    return DateTime.ParseExact(timeString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                case DateTimeFormat.OnlyTime:
                {
                    return DateTime.ParseExact(timeString, "HH:mm:ss", CultureInfo.InvariantCulture);
                }
                case DateTimeFormat.DateAndTime:
                {
                    return DateTime.ParseExact(timeString, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                default:
                    throw new Exception("Chưa code định dạng này");
            }
        }

        #endregion


        #region TimeSpan

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            long days = (long)timeSpan.TotalDays;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (days > 0)
                return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", days, hours, minutes, seconds);

            if (hours > 0)
                return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        #endregion

        #region Float

        public static float StringToFloat(string s)
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }

        public static bool TryStringToFloat(string s, out float result)
        {
            try
            {
                result = float.Parse(s, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                result = 0f;
                return false;
            }
        }

        public static string FloatToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string FormatMoney(float money)
        {
            return Format("{0:0.#######}", money);
        }

        #endregion

        #region Decimal

        public static decimal StringToDecimal(string s)
        {
            return decimal.Parse(s, CultureInfo.InvariantCulture);
        }

        public static bool TryStringToDecimal(string s, out decimal result)
        {
            try
            {
                result = decimal.Parse(s, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                result = 0m;
                return false;
            }
        }

        public static string DecimalToString(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}