using UnityEngine;

namespace ZeroX.Extensions
{
    public static class Vector3Extension
    {
        public static bool Approximately(this Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }
        
        public static bool IsApproximatelyZero(this Vector3 a)
        {
            return Mathf.Approximately(a.x, 0) && Mathf.Approximately(a.y, 0) && Mathf.Approximately(a.z, 0);
        }
        
        
        
        public static Vector3 ProjectPointOnDirection(this Vector3 point, Vector3 pointOnDirection, Vector3 direction)
        {
            Vector3 canhHuyen = point - pointOnDirection;
            float angle = Vector3.Angle(canhHuyen, direction);
            float doDaiCanhKe = canhHuyen.magnitude * Mathf.Cos(angle * Mathf.Deg2Rad);
            return pointOnDirection + direction.normalized * doDaiCanhKe;
        }
    }
}