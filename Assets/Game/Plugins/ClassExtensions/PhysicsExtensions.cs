using System;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsExtensions
{
    public static Collider[] OverlapSphere(Vector3 center, float radius, bool draw)
    {
#if UNITY_EDITOR
        if(draw) DebugDrawSphere(center, radius);
#endif
        return Physics.OverlapSphere(center, radius);
    }
    
    private static void DebugDrawSphere(Vector3 center, float radius, float duration = 3f, int segments = 16)
    {
	    DrawCircle(center, Vector3.forward, Vector3.right, radius, Color.blue,    duration, segments); // XY
	    DrawCircle(center, Vector3.forward, Vector3.up,    radius, Color.green,   duration, segments); // XZ
	    DrawCircle(center, Vector3.up,      Vector3.right, radius, Color.red, duration, segments); // YZ
    }
    
    private static void DrawCircle(Vector3 center, Vector3 axis1, Vector3 axis2, float radius, Color color, float duration, int segments)
    {
	    float angleStep = 360f / segments;
	    
	    for (int i = 0; i < segments; i++)
	    {
		    float angle1 = Mathf.Deg2Rad * (i                  * angleStep);
		    float angle2 = Mathf.Deg2Rad * ((i + 1) % segments * angleStep);
		    
		    Vector3 point1 = center + (Mathf.Cos(angle1) * axis1 + Mathf.Sin(angle1) * axis2) * radius;
		    Vector3 point2 = center + (Mathf.Cos(angle2) * axis1 + Mathf.Sin(angle2) * axis2) * radius;
		    
		    Debug.DrawLine(point1, point2, color, duration);
	    }
    }
}
