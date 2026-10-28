using System;
using System.Globalization;
using UnityEngine;

namespace ZeroX
{
    [System.Serializable]
    public struct ZNumber : ISerializationCallbackReceiver
    {
        [SerializeField] private string seriDecimalPart; //serialization
        [SerializeField] private long magnitude;
        
        
        
        //Const
        private const int Max_Magnitude_Offset_To_Calculate = 28;
        
        
        
        //Runtime
        private decimal decimalPart;
        
        
        
        
        
        
        
        public void OnBeforeSerialize()
        {
            seriDecimalPart = decimalPart.ToString(CultureInfo.InvariantCulture);
        }

        public void OnAfterDeserialize()
        {
            decimalPart = decimal.Parse(seriDecimalPart, CultureInfo.InvariantCulture);
        }


        #region Constructor

        public ZNumber(decimal decimalPart, long magnitude)
        {
            this.decimalPart = decimalPart;
            this.magnitude = magnitude;

            this.seriDecimalPart = null; //Chỉ thay dổi bởi Serialize của unity
        }

        #endregion
        
        

        #region Math
        
        public static decimal Pow(decimal baseValue, long exponent)
        {
            if (exponent == 0)
                return 1m;

            if (baseValue == 0m)
            {
                if (exponent < 0)
                    throw new DivideByZeroException();
                
                return 0m;
            }

            //Không cần vì số mũ mà to hoặc nhỏ quá thì decimal cũng ko biểu diễn được :)))
            // if (exponent == long.MinValue)
            //     throw new OverflowException("Exponent too small.");


            //Adjust Exponent
            bool isNegative = exponent < 0;
            exponent = isNegative ? -exponent : exponent;
            
            
            decimal result = 1m;
            decimal factor = baseValue;
            while (exponent > 0)
            {
                if (exponent % 2 != 0)
                    result *= factor;

                factor *= factor;
                exponent >>= 1; //Chính là chia cho 2 và làm tròn xuống, dùng dịch bit thì nhanh hơn
            }

            return isNegative ? 1 / result : result;
        }

        public static long CalculateMagnitude(decimal number)
        {
            string s = number.ToString(CultureInfo.InvariantCulture);
            int dotIndex = s.IndexOf('.');
            if (dotIndex == -1)
                return s.Length - 1;
            else
                return dotIndex - 1;
        }
        
        #endregion
        

        
        #region Magnitude

        /// <summary>
        /// Lưu ý độ lớn không thể nhỏ hơn 0. Và nếu chênh lệch magnitude quá lớn sẽ khiến số trở thành 0
        /// </summary>
        public void ChangeMagnitude(long newMagnitude)
        {
            if(newMagnitude < 0)
                throw new Exception("Magnitude cannot be negative");
            
            decimalPart /= Pow(10, newMagnitude - magnitude);
            magnitude = newMagnitude;
        }
        
        /// <summary>
        /// Chuẩn hóa độ lớn sao cho decimal value chỉ ở dạng 1.xxx hoặc 0.xxx
        /// </summary>
        public void NormalizeMagnitude()
        {
            long additionMagnitude = CalculateMagnitude(decimalPart);
            ChangeMagnitude(magnitude + additionMagnitude);
        }

        #endregion







        #region Operator

        public static ZNumber operator +(ZNumber a, ZNumber b)
        {
            if (a.magnitude >= b.magnitude)
            {
                b.ChangeMagnitude(a.magnitude);
            }
            else
            {
                a.ChangeMagnitude(b.magnitude);
            }

            a.decimalPart += b.decimalPart;
            a.NormalizeMagnitude();
            return a;
        }
        
        public static ZNumber operator -(ZNumber a, ZNumber b)
        {
            if (a.magnitude >= b.magnitude)
            {
                b.ChangeMagnitude(a.magnitude);
            }
            else
            {
                a.ChangeMagnitude(b.magnitude);
            }

            a.decimalPart -= b.decimalPart;
            a.NormalizeMagnitude();
            return a;
        }
        
        public static ZNumber operator *(ZNumber a, ZNumber b)
        {
            a.decimalPart *= b.decimalPart;
            a.magnitude += b.magnitude;
            a.NormalizeMagnitude();
            return a;
        }
        
        public static ZNumber operator /(ZNumber a, ZNumber b)
        {
            ZNumber result = new ZNumber();
            
            if (a.magnitude >= b.magnitude)
            {
                result.decimalPart = a.decimalPart / b.decimalPart;
                result.magnitude = a.magnitude - b.magnitude;
            }
            else
            {
                long magnitudeOffset = b.magnitude - a.magnitude;
                result.decimalPart = (a.decimalPart / b.decimalPart) / Pow(10, magnitudeOffset);
                result.magnitude = 0;
            }
            
            result.NormalizeMagnitude();
            return result;
        }

        #endregion
        
    }
}