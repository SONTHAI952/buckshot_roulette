
using System;
using System.Text;

namespace ZeroX.MathSystem
{
    public static class MathBig
    {
        const int MAX_DECIMAL_DIGITS = 10;

        public static string Pow(string baseValue, string exponent)
        {
            if (exponent == "0") return "1";
            if (exponent == "1") return baseValue;

            if (exponent.Contains("."))
            {
                // Dùng phương pháp x^y = exp(y * ln(x))
                string lnBase = Ln(baseValue);
                string product = Multiply(lnBase, exponent);
                return Exp(product);
            }
            else
            {
                return IntegerPow(baseValue, exponent);
            }
        }

        private static string IntegerPow(string baseValue, string exponent)
        {
            string result = "1";
            string count = "0";
            while (Compare(count, exponent) < 0)
            {
                result = Multiply(result, baseValue);
                count = Add(count, "1");
            }
            return result;
        }

        public static string Add(string a, string b)
        {
            string[] partsA = a.Split('.');
            string[] partsB = b.Split('.');
            string intA = partsA[0];
            string intB = partsB[0];
            string decA = partsA.Length > 1 ? partsA[1] : "";
            string decB = partsB.Length > 1 ? partsB[1] : "";

            int maxDecimalLength = Math.Max(decA.Length, decB.Length);
            decA = decA.PadRight(maxDecimalLength, '0');
            decB = decB.PadRight(maxDecimalLength, '0');

            string decSum = AddIntegerStrings(decA, decB);
            bool carry = decSum.Length > maxDecimalLength;
            if (carry)
                decSum = decSum.Substring(1);

            string intSum = AddIntegerStrings(intA, intB);
            if (carry)
                intSum = AddIntegerStrings(intSum, "1");

            return TrimZeros(intSum + "." + decSum);
        }

        public static string Multiply(string a, string b)
        {
            int decA = a.Contains(".") ? a.Length - a.IndexOf('.') - 1 : 0;
            int decB = b.Contains(".") ? b.Length - b.IndexOf('.') - 1 : 0;

            a = a.Replace(".", "");
            b = b.Replace(".", "");

            string product = MultiplyIntegerStrings(a, b);
            int totalDecimals = decA + decB;

            if (totalDecimals > 0)
            {
                product = product.PadLeft(totalDecimals + 1, '0');
                product = product.Insert(product.Length - totalDecimals, ".");
            }

            return TrimZeros(TruncateDecimal(product, MAX_DECIMAL_DIGITS));
        }

        public static string Divide(string numerator, string denominator, int precision = MAX_DECIMAL_DIGITS)
        {
            // B1: Xác định số chữ số thập phân
            int numDecimalPlaces = numerator.Contains(".") ? numerator.Length - numerator.IndexOf('.') - 1 : 0;
            int denomDecimalPlaces = denominator.Contains(".") ? denominator.Length - denominator.IndexOf('.') - 1 : 0;

            // B2: Xoá dấu chấm
            numerator = numerator.Replace(".", "");
            denominator = denominator.Replace(".", "");

            // B3: Bù phần thập phân thành số nguyên
            int scale = denomDecimalPlaces - numDecimalPlaces;
            if (scale > 0)
                numerator = numerator + new string('0', scale);
            else if (scale < 0)
                denominator = denominator + new string('0', -scale);

            // B4: Thêm thêm số 0 để đảm bảo phần thập phân chính xác
            numerator = numerator + new string('0', precision);

            // B5: Chia như số nguyên
            string rawResult = IntegerDivide(numerator, denominator);

            // B6: Thêm dấu chấm vào đúng vị trí
            int decimalIndex = rawResult.Length - precision;
            if (decimalIndex <= 0)
            {
                rawResult = rawResult.PadLeft(precision + 1, '0');
                decimalIndex = 1;
            }

            rawResult = rawResult.Insert(decimalIndex, ".");

            return TrimZeros(rawResult);
        }
        
        public static string IntegerDivide(string num, string denom)
        {
            StringBuilder result = new StringBuilder();
            string remainder = "0";

            foreach (char digit in num)
            {
                remainder = AddIntegerStrings(MultiplyIntegerStrings(remainder, "10"), digit.ToString());

                int count = 0;
                while (Compare(remainder, denom) >= 0)
                {
                    remainder = SubtractIntegerStrings(remainder, denom);
                    count++;
                }

                result.Append(count);
            }

            return result.ToString().TrimStart('0');
        }

        public static string Ln(string x)
        {
            // ln(x) bằng chuỗi Taylor (x-1)-(x-1)^2/2+(x-1)^3/3... cho x gần 1
            // cần chuyển đổi x về gần 1 bằng phép đổi biến: ln(x) = ln(a^n) = n * ln(a)
            throw new NotImplementedException("Ln function is placeholder");
        }

        public static string Exp(string x)
        {
            // Tương tự, dùng chuỗi Taylor cho e^x = 1 + x + x^2/2! + ...
            throw new NotImplementedException("Exp function is placeholder");
        }

        private static string AddIntegerStrings(string a, string b)
        {
            StringBuilder result = new StringBuilder();
            int carry = 0;
            int i = a.Length - 1, j = b.Length - 1;
            while (i >= 0 || j >= 0 || carry > 0)
            {
                int digitA = i >= 0 ? a[i--] - '0' : 0;
                int digitB = j >= 0 ? b[j--] - '0' : 0;
                int sum = digitA + digitB + carry;
                result.Insert(0, sum % 10);
                carry = sum / 10;
            }
            return result.ToString();
        }

        private static string SubtractIntegerStrings(string a, string b)
        {
            // Giả sử a >= b
            StringBuilder result = new StringBuilder();
            int i = a.Length - 1, j = b.Length - 1, borrow = 0;
            while (i >= 0)
            {
                int digitA = a[i] - '0';
                int digitB = j >= 0 ? b[j] - '0' : 0;
                int diff = digitA - digitB - borrow;
                if (diff < 0)
                {
                    diff += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }
                result.Insert(0, diff);
                i--; j--;
            }
            return result.ToString().TrimStart('0');
        }

        private static string MultiplyIntegerStrings(string a, string b)
        {
            int[] result = new int[a.Length + b.Length];
            for (int i = a.Length - 1; i >= 0; i--)
            {
                for (int j = b.Length - 1; j >= 0; j--)
                {
                    int mul = (a[i] - '0') * (b[j] - '0');
                    int p1 = i + j, p2 = i + j + 1;
                    int sum = mul + result[p2];
                    result[p2] = sum % 10;
                    result[p1] += sum / 10;
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (int num in result)
            {
                if (!(sb.Length == 0 && num == 0))
                    sb.Append(num);
            }

            return sb.Length == 0 ? "0" : sb.ToString();
        }

        private static string TrimZeros(string num)
        {
            if (!num.Contains(".")) return num.TrimStart('0').Length == 0 ? "0" : num.TrimStart('0');
            string[] parts = num.Split('.');
            string intPart = parts[0].TrimStart('0');
            string decPart = parts[1].TrimEnd('0');
            if (intPart == "") intPart = "0";
            return decPart == "" ? intPart : intPart + "." + decPart;
        }

        private static string TruncateDecimal(string num, int digits)
        {
            if (!num.Contains(".")) return num;
            int dot = num.IndexOf('.');
            return num.Substring(0, Math.Min(num.Length, dot + 1 + digits));
        }

        private static int Compare(string a, string b)
        {
            a = TrimZeros(a);
            b = TrimZeros(b);
            if (!a.Contains(".")) a += ".0";
            if (!b.Contains(".")) b += ".0";
            string[] pa = a.Split('.');
            string[] pb = b.Split('.');

            int cmp = pa[0].Length.CompareTo(pb[0].Length);
            if (cmp != 0) return cmp;
            cmp = String.Compare(pa[0], pb[0], StringComparison.InvariantCulture);
            if (cmp != 0) return cmp;

            return String.Compare(pa[1].PadRight(MAX_DECIMAL_DIGITS, '0'), pb[1].PadRight(MAX_DECIMAL_DIGITS, '0'), StringComparison.InvariantCulture);
        }
        
        public static string Round(string num, int digits = 0)
        {
            num = TrimZeros(num);

            if (!num.Contains("."))
                return num;

            string[] parts = num.Split('.');
            string intPart = parts[0];
            string decPart = parts.Length > 1 ? parts[1] : "";

            // Nếu chuỗi quá ngắn, thêm 0 cho đủ
            if (decPart.Length <= digits)
            {
                decPart = decPart.PadRight(digits, '0');
                return TrimZeros(intPart + "." + decPart);
            }

            // Xác định chữ số kế tiếp (để làm tròn)
            int roundDigit = decPart[digits] - '0';

            // Cắt chuỗi đến phần cần giữ
            decPart = decPart.Substring(0, digits);

            if (roundDigit >= 5)
            {
                // Tăng phần thập phân
                string increased = AddIntegerStrings(decPart, "1");

                // Nếu bị vượt độ dài (ví dụ 999 + 1 = 1000), xử lý overflow
                if (increased.Length > digits)
                {
                    increased = increased.Substring(1); // bỏ chữ số dư
                    intPart = AddIntegerStrings(intPart, "1");
                }

                decPart = increased.PadLeft(digits, '0');
            }

            return digits == 0 ? intPart : TrimZeros(intPart + "." + decPart);
        }
    }
}