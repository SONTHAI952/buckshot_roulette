using UnityEngine;

public static class FloatExtensions
{
	public static float ClampBetween(this float value, float a, float b)
	{
		return Mathf.Clamp(value, Mathf.Min(a, b), Mathf.Max(a, b));
	}
}