using System.Globalization;
using System.Numerics;

namespace DataSystem.Money
{
    public static class MoneyFormat
    {
        public static string Format(decimal amount)
        {
            return amount.ToString();
            //return amount.Abbreviated();
        }
    }
}