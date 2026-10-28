using UnityEngine;

namespace ZeroX.Extensions
{
    public static class FloatExtension
    {
        public static bool LessThanOrEqual(this float a, float value)
        {
            return a < value || Mathf.Approximately(a, value);
        }
        
        public static bool LessThanOrEqualZero(this float a)
        {
            return a < 0 || Mathf.Approximately(a, 0f);
        }
        
        public static bool GreaterThanOrEqual(this float a, float value)
        {
            return a > value || Mathf.Approximately(a, value);
        }
        
        public static bool GreaterThanOrEqualZero(this float a)
        {
            return a > 0 || Mathf.Approximately(a, 0f);
        }
    }
}