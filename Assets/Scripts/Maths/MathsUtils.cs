using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsUtils
{
	public static Vector3 LinearBezier(Vector3 A, Vector3 B, float t)
	{
		if (t > 1)
			t = 1;
		return A + ((B - A) * t);
	}
	public static Vector3 QuadraticBezier(Vector3 A, Vector3 B, Vector3 C, float t)
	{
		if (t > 1)
			t = 1;
		float a = 1 - t;
		Vector3 bezierPoint = (a * a * A) +  (2*a*t*B)  + (t*t * C);
		return bezierPoint;
	}

	public static Vector3 CubicBezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
	{
		if (t > 1)
			t = 1;
		float a = 1 - t;
		Vector3 bezierPoint = (Mathf.Pow(a, 3) * A) + 
							  (3 * Mathf.Pow(a, 2) * Mathf.Pow(t, 1) * B) +
							  (3 * Mathf.Pow(a, 1) * Mathf.Pow(t, 2) * C) + 
							  (Mathf.Pow(t, 3) * D);
		return bezierPoint;
	}
}
