using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Curve
{
	public Vector3 A;
	public Vector3 B;
	public Vector3 C;
	public Vector3 D;

	public Vector3 GetPosition(float t)
	{
		return MathsUtils.CubicBezier(A, B, C, D, t);
	}

	public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix)
	{
		Vector3 pWorld = localToWorldMatrix.MultiplyPoint(MathsUtils.CubicBezier(A, B, C, D, t));
		//pWorld.
		return pWorld;
	}

	public void DrawGizmo(Color c, Matrix4x4 localToWorldMatrix)
	{
		Gizmos.color = c;
		Vector3 pA = localToWorldMatrix.MultiplyPoint(A);
		Vector3 pB = localToWorldMatrix.MultiplyPoint(B);
		Vector3 pC = localToWorldMatrix.MultiplyPoint(C);
		Vector3 pD = localToWorldMatrix.MultiplyPoint(D);
		Gizmos.DrawSphere(pA, 0.25f);
		Gizmos.DrawSphere(pB, 0.25f);
		Gizmos.DrawSphere(pC, 0.25f);
		Gizmos.DrawSphere(pD, 0.25f);
		Vector3 position = pA;
		for (int i = 1; i < 10; i++)
		{
			Vector3 nextPosition = GetPosition(1f * (i / 10f), localToWorldMatrix);
			Gizmos.DrawLine(position, nextPosition);
			position = nextPosition;
		}
		//Gizmos.DrawLine(pivot, position);
		/*Gizmos.matrix = Matrix4x4.TRS(position, GetRotation(), Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, fieldOfView, 0.5f, 0f, Camera.main.aspect);
		Gizmos.matrix = Matrix4x4.identity;*/
	}
}
