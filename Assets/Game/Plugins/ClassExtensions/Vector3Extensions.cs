
using UnityEngine;

public static class Vector3Extensions
{
    public static float DistanceSquared(this Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }
    
    public static bool ApproximatelyEquals(this Vector3 a, Vector3 b, float epsilon = 0.1f)
    {
        return Vector3.SqrMagnitude(a - b) < epsilon * epsilon;
    }
    
    public static bool IsWithinDistance(this Vector3 a, Vector3 b, float distance)
    {
        return a.DistanceSquared(b) < distance * distance;
    }
    
    public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
    {
        return new Vector3(
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y),
                Mathf.Clamp(value.z, min.z, max.z)
        );
    }
    
    public static Vector3 Clamp(this Vector3 value, float min, float max)
    {
        return new Vector3(
                Mathf.Clamp(value.x, min, max),
                Mathf.Clamp(value.y, min, max),
                Mathf.Clamp(value.z, min, max)
        );
    }
    
    public static Vector3 ToVector3(this int value)
    {
        float f = value;
        return new Vector3(f, f, f);
    }
    
    public static Vector3 ToVector3(this float value)
    {
        return new Vector3(value, value, value);
    }
    
    public static Vector3 ToVector3(this double value)
    {
        float f = (float)value;
        return new Vector3(f, f, f);
    }
}
