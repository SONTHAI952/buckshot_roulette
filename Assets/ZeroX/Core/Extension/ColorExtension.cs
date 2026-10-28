using UnityEngine;

namespace ZeroX.Extensions
{
    public static class ColorExtension
    {
        public static bool IsApproximately(this Color color, Color otherColor, float epsilon)
        {
            if (Mathf.Abs(color.r - otherColor.r) > epsilon)
                return false;
            
            if (Mathf.Abs(color.g - otherColor.g) > epsilon)
                return false;
            
            if (Mathf.Abs(color.b - otherColor.b) > epsilon)
                return false;
            
            if (Mathf.Abs(color.a - otherColor.a) > epsilon)
                return false;

            return true;
        }
        
        public static bool IsApproximately(this Color color, Color otherColor)
        {
            if (Mathf.Abs(color.r - otherColor.r) > 0.0001f)
                return false;
            
            if (Mathf.Abs(color.g - otherColor.g) > 0.0001f)
                return false;
            
            if (Mathf.Abs(color.b - otherColor.b) > 0.0001f)
                return false;
            
            if (Mathf.Abs(color.a - otherColor.a) > 0.0001f)
                return false;

            return true;
        }

        /// <summary>
        /// AddValue là dạng 0-1. Giá trị dương là tăng độ sáng, âm là giảm độ sáng
        /// </summary>
        public static Color AddBrightness(Color color, float addValue)
        {
            //RGB
            return new Color(color.r + addValue, color.g + addValue, color.b + addValue);
        }
    }
}