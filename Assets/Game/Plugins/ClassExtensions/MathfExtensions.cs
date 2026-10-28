
using UnityEngine;

public static class MathfExtensions
{
	public static float ClampLoop(this float value, float min, float max)
	{
		if (value > max) return min;
		if (value < min) return max;
		return value;
	}
	
	public static int ClampLoop(this int value, int min, int max)
	{
		if (value > max) return min;
		if (value < min) return max;
		return value;
	}
	
	public static int RoundFloatToNearestMultipleOf3(float value)
	{
		return Mathf.RoundToInt(value / 3f) * 3;
	}
	
	public static int RoundFloatToNearestMultipleOf2(float value)
	{
		return Mathf.RoundToInt(value / 2f) * 2;
	}
}