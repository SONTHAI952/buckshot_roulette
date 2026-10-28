using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace DataSystem.Money
{
    public enum MoneyType
    {
        Null = -1, Coin = 0, Gem = 1
    }

    public static class MoneyExtension
    {
        static Dictionary<string, MoneyType> dictStringToType = new Dictionary<string, MoneyType>()
        {
            {"coin", MoneyType.Coin},
            {"gem", MoneyType.Gem}
        };

        public static MoneyType FromString(string moneyType)
        {
            moneyType = moneyType.ToLower();
            if (dictStringToType.TryGetValue(moneyType, out var t) == false)
            {
                Debug.LogError("Chưa code loại money này: " + moneyType);
                return MoneyType.Null;
            }

            return t;
        }

        public static string GetTextIcon(this MoneyType moneyType)
        {
            switch (moneyType)
            {
                case MoneyType.Coin:
                    return "<sprite=\"coin\" index=0>";
                case MoneyType.Gem:
                    return "<sprite=\"gem\" index=0>";
                default: return " Null";
            }
        }

        static string StringFormat(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        public static string ToString(MoneyType moneyType, decimal amount, bool thousandsSeparator = true)
        {
            if(thousandsSeparator)
                return StringFormat("{0:#,##0.##}", amount);
            else
                return StringFormat("{0:0.##}", amount);
        }
        
        public static string ToStringWithIcon(MoneyType moneyType, decimal amount, bool thousandsSeparator = true)
        {
            if(thousandsSeparator)
                return StringFormat("{0:#,##0.##}{1}", amount, moneyType.GetTextIcon());
            else
                return StringFormat("{0:0.##}{1}", amount, moneyType.GetTextIcon());
        }
        
        public static string ToStringWithIconReverse(MoneyType moneyType, decimal amount, bool thousandsSeparator = true)
        {
            if(thousandsSeparator)
                return StringFormat("{1}{0:#,##0.##}", amount, moneyType.GetTextIcon());
            else
                return StringFormat("{1}{0:0.##}", amount, moneyType.GetTextIcon());
        }
    }
}