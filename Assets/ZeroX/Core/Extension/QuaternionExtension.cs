using UnityEngine;

namespace ZeroX.Extensions
{
    public static class QuaternionExtension
    {
        public static Quaternion ClampAroundAxisX(this Quaternion q, float minEulerAngel, float maxEulerAngel)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, minEulerAngel, maxEulerAngel);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public static Quaternion ClampAroundAxisY(this Quaternion q, float minEulerAngel, float maxEulerAngel)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angelY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
            angelY = Mathf.Clamp(angelY, minEulerAngel, maxEulerAngel);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angelY);

            return q;
        }

        public static Quaternion ClampAroundAxisZ(this Quaternion q, float minEulerAngel, float maxEulerAngel)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angelZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
            angelZ = Mathf.Clamp(angelZ, minEulerAngel, maxEulerAngel);
            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angelZ);

            return q;
        }

        public static Quaternion CalculateOffset(Quaternion from, Quaternion to)
        {
            return Quaternion.Inverse(from) * to;
        }
    }
}