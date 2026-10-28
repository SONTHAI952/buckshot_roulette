using UnityEngine;

namespace ZeroX.Extensions
{
    public static class Vector2Extension
    {
        public static bool Approximately(this Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }
        
        public static bool IsApproximatelyZero(this Vector2 a)
        {
            return Mathf.Approximately(a.x, 0) && Mathf.Approximately(a.y, 0);
        }
    }
}