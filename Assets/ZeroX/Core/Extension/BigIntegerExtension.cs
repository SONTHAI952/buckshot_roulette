using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZeroX.Extensions
{
    public static class BigIntegerExtension
    {
        public static void GetFraction(decimal value, out BigInteger numerator, out BigInteger denominator)
        {
            Decimal d = value;
            int[] bits = decimal.GetBits(d);
            numerator = (1 - ((bits[3] >> 30) & 2)) *
                        unchecked(((BigInteger) (uint) bits[2] << 64) |
                                  ((BigInteger) (uint) bits[1] << 32) |
                                  (BigInteger) (uint) bits[0]);

            denominator = BigInteger.Pow(10, (bits[3] >> 16) & 0xff);
        }


        static void HandleNumberString(string numberString, int decimals, out string integerPartString,
            out string decimalPartString)
        {
            if (numberString.Length > decimals)
            {
                integerPartString = numberString.Substring(0, numberString.Length - decimals);
                decimalPartString = numberString.Substring(numberString.Length - decimals, decimals).TrimEnd('0');
            }
            else
            {
                integerPartString = "0";

                int miss = decimals - numberString.Length;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < miss; i++)
                {
                    sb.Append("0");
                }

                sb.Append(numberString);

                decimalPartString = sb.ToString().TrimEnd('0');
            }
        }


        #region Multiply

        public static void Multiply(this BigInteger number, decimal multiplier, int decimals,
            out string integerPartString, out string decimalPartString)
        {
            for (int i = 0; i < decimals; i++)
            {
                number *= 10;
            }

            GetFraction(multiplier, out var numerator, out var denominator);
            number = BigInteger.Multiply(number, numerator);
            number = BigInteger.Divide(number, denominator);

            HandleNumberString(number.ToString(), decimals, out integerPartString, out decimalPartString);
        }

        public static BigInteger MultiplyRound(this BigInteger number, decimal multiplier)
        {
            Multiply(number, multiplier, 1, out string integerPartString, out string decimalPartString);


            int firstDecimalPart = 0;
            if (string.IsNullOrEmpty(decimalPartString) == false)
                firstDecimalPart = int.Parse(decimalPartString[0].ToString());


            if (firstDecimalPart == 0)
                return BigInteger.Parse(integerPartString);

            if (firstDecimalPart < 5)
                return BigInteger.Parse(integerPartString) - 1;

            return BigInteger.Parse(integerPartString) + 1;
        }

        public static BigInteger MultiplyFloor(this BigInteger number, decimal multiplier)
        {
            GetFraction(multiplier, out var numerator, out var denominator);
            number = BigInteger.Multiply(number, numerator);
            return BigInteger.Divide(number, denominator);
        }

        public static BigInteger MultiplyCeil(this BigInteger number, decimal multiplier)
        {
            Multiply(number, multiplier, 1, out string integerPartString, out string decimalPartString);


            int firstDecimalPart = 0;
            if (string.IsNullOrEmpty(decimalPartString) == false)
                firstDecimalPart = int.Parse(decimalPartString[0].ToString());


            if (firstDecimalPart == 0)
                return BigInteger.Parse(integerPartString);

            return BigInteger.Parse(integerPartString) + 1;
        }


        #endregion


        #region Divide

        public static void Divide(this BigInteger number, decimal divisor, int decimals, out string integerPartString,
            out string decimalPartString)
        {
            for (int i = 0; i < decimals; i++)
            {
                number *= 10;
            }

            GetFraction(divisor, out var numerator, out var denominator);
            number = BigInteger.Multiply(number, denominator);
            number = BigInteger.Divide(number, numerator);

            HandleNumberString(number.ToString(), decimals, out integerPartString, out decimalPartString);
        }

        public static void Divide(this BigInteger number, BigInteger divisor, int decimals,
            out string integerPartString, out string decimalPartString)
        {
            for (int i = 0; i < decimals; i++)
            {
                number *= 10;
            }

            number = BigInteger.Divide(number, divisor);

            HandleNumberString(number.ToString(), decimals, out integerPartString, out decimalPartString);
        }



        public static BigInteger DivideRound(this BigInteger number, decimal divisor)
        {
            Divide(number, divisor, 1, out string integerPartString, out string decimalPartString);


            int firstDecimalPart = 0;
            if (string.IsNullOrEmpty(decimalPartString) == false)
                firstDecimalPart = int.Parse(decimalPartString[0].ToString());


            if (firstDecimalPart == 0)
                return BigInteger.Parse(integerPartString);

            if (firstDecimalPart < 5)
                return BigInteger.Parse(integerPartString) - 1;

            return BigInteger.Parse(integerPartString) + 1;
        }

        public static BigInteger DivideFloor(this BigInteger number, decimal divisor)
        {
            GetFraction(divisor, out var numerator, out var denominator);
            number = BigInteger.Multiply(number, denominator);
            return BigInteger.Divide(number, numerator);
        }

        public static BigInteger DivideCeil(this BigInteger number, decimal divisor)
        {
            Divide(number, divisor, 1, out string integerPartString, out string decimalPartString);


            int firstDecimalPart = 0;
            if (string.IsNullOrEmpty(decimalPartString) == false)
                firstDecimalPart = int.Parse(decimalPartString[0].ToString());


            if (firstDecimalPart == 0)
                return BigInteger.Parse(integerPartString);

            return BigInteger.Parse(integerPartString) + 1;
        }



        public static BigInteger DivideRound(this BigInteger number, BigInteger divisor)
        {
            Divide(number, divisor, 1, out string integerPartString, out string decimalPartString);


            int firstDecimalPart = 0;
            if (string.IsNullOrEmpty(decimalPartString) == false)
                firstDecimalPart = int.Parse(decimalPartString[0].ToString());


            if (firstDecimalPart == 0)
                return BigInteger.Parse(integerPartString);

            if (firstDecimalPart < 5)
                return BigInteger.Parse(integerPartString) - 1;

            return BigInteger.Parse(integerPartString) + 1;
        }

        public static BigInteger DivideFloor(this BigInteger number, BigInteger divisor)
        {
            return BigInteger.Divide(number, divisor);
        }

        public static BigInteger DivideCeil(this BigInteger number, BigInteger divisor)
        {
            Divide(number, divisor, 1, out string integerPartString, out string decimalPartString);


            int firstDecimalPart = 0;
            if (string.IsNullOrEmpty(decimalPartString) == false)
                firstDecimalPart = int.Parse(decimalPartString[0].ToString());


            if (firstDecimalPart == 0)
                return BigInteger.Parse(integerPartString);

            return BigInteger.Parse(integerPartString) + 1;
        }

        #endregion



        #region Abbreviated

        private class AbbreviatedData
        {
            public BigInteger number;
            public string symbol;

            public AbbreviatedData()
            {
            }

            public AbbreviatedData(string number, string symbol)
            {
                this.number = BigInteger.Parse(number);
                this.symbol = symbol;
            }
        }

        private static readonly List<AbbreviatedData> listAbbreviated = new List<AbbreviatedData>()
        {
            new AbbreviatedData("1000", "K"),
            new AbbreviatedData("1000000", "M"),
            new AbbreviatedData("1000000000", "B"),
            new AbbreviatedData("1000000000000", "T"),
        };

        static void AbbreviatedAnalytics(BigInteger number, out string integerPart, out string decimalPart,
            out string abbreviatedSymbol)
        {
            if (number < listAbbreviated[0].number)
            {
                integerPart = number.ToString();
                decimalPart = "";
                abbreviatedSymbol = "";
                return;
            }

            for (int i = 1; i < listAbbreviated.Count; i++)
            {
                var data = listAbbreviated[i];
                var prevData = listAbbreviated[i - 1];
                if (number < data.number)
                {
                    number.Divide(prevData.number, 1, out integerPart, out decimalPart);
                    abbreviatedSymbol = prevData.symbol;
                    return;
                }
            }

            var lastData = listAbbreviated[listAbbreviated.Count - 1];
            number.Divide(lastData.number, 1, out integerPart, out decimalPart);
            abbreviatedSymbol = lastData.symbol;
        }

        public static string Abbreviated(this BigInteger number)
        {
            string ConnectPart(string integerP, string decimalP, string abbreviatedChar)
            {
                if (string.IsNullOrEmpty(decimalP))
                    return integerP + abbreviatedChar;
                else
                    return integerP + "." + decimalP + abbreviatedChar;
            }


            AbbreviatedAnalytics(number, out string integerPart, out string decimalPart, out string abbreviatedSymbol);
            BigInteger integerPartNumber = BigInteger.Parse(integerPart);
            if (integerPartNumber < 999)
                return ConnectPart(integerPart, decimalPart, abbreviatedSymbol);



            StringBuilder abbreviatedSymbolSb = new StringBuilder(abbreviatedSymbol);
            while (integerPartNumber >= 1000)
            {
                AbbreviatedAnalytics(integerPartNumber, out integerPart, out decimalPart, out abbreviatedSymbol);
                integerPartNumber = BigInteger.Parse(integerPart);
                abbreviatedSymbolSb.Insert(0, abbreviatedSymbol);
            }

            return ConnectPart(integerPart, decimalPart, abbreviatedSymbolSb.ToString());
        }

        #endregion
    }
}