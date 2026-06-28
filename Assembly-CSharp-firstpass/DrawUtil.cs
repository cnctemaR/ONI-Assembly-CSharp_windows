using System;
using System.Diagnostics;
using UnityEngine;

public static class DrawUtil
{
	[Conditional("ENABLE_DEBUG_OUTPUT")]
	public static void Gnomon(Vector3 pos, float size)
	{
		size *= 0.5f;
		global::UnityEngine.Debug.DrawLine(pos - Vector3.right * size, pos + Vector3.right * size, Color.red);
		global::UnityEngine.Debug.DrawLine(pos - Vector3.up * size, pos + Vector3.up * size, Color.green);
		global::UnityEngine.Debug.DrawLine(pos - Vector3.forward * size, pos + Vector3.forward * size, Color.blue);
	}

	[Conditional("ENABLE_DEBUG_OUTPUT")]
	public static void Gnomon(Vector3 pos, float size, Color color, float time = 0f)
	{
		size *= 0.5f;
		global::UnityEngine.Debug.DrawLine(pos - Vector3.right * size, pos + Vector3.right * size, color, time);
		global::UnityEngine.Debug.DrawLine(pos - Vector3.up * size, pos + Vector3.up * size, color, time);
		global::UnityEngine.Debug.DrawLine(pos - Vector3.forward * size, pos + Vector3.forward * size, color, time);
	}

	[Conditional("ENABLE_DEBUG_OUTPUT")]
	public static void Arrow(Vector3 start, Vector3 end, float size, Color color, float time = 0f)
	{
		Vector3 vector = end - start;
		if (vector.sqrMagnitude < 0.001f)
		{
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(vector, Vector3.up);
		global::UnityEngine.Debug.DrawLine(start, end, color, time);
		global::UnityEngine.Debug.DrawLine(end, end + quaternion * new Vector3(-size, 0f, -size), color, time);
		global::UnityEngine.Debug.DrawLine(end, end + quaternion * new Vector3(size, 0f, -size), color, time);
		global::UnityEngine.Debug.DrawLine(end, end + quaternion * new Vector3(0f, -size, -size), color, time);
		global::UnityEngine.Debug.DrawLine(end, end + quaternion * new Vector3(0f, size, -size), color, time);
	}

	[Conditional("ENABLE_DEBUG_OUTPUT")]
	public static void Circle(Vector3 pos, float radius)
	{
		DrawUtil.Circle(pos, radius, Color.white, null, 0f);
	}

	[Conditional("ENABLE_DEBUG_OUTPUT")]
	public static void Circle(Vector3 pos, float radius, Color color, Vector3? normal = null, float time = 0f)
	{
		Vector3 vector = ((normal == null) ? Vector3.up : normal.Value);
		int num = 40;
		if (DrawUtil.circlePointCache == null)
		{
			float num2 = 6.2831855f / (float)num;
			DrawUtil.circlePointCache = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				DrawUtil.circlePointCache[i] = new Vector3(Mathf.Cos(num2 * (float)i), Mathf.Sin(num2 * (float)i), 0f);
			}
		}
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, vector);
		for (int j = 0; j < num - 1; j++)
		{
			global::UnityEngine.Debug.DrawLine(pos + quaternion * (radius * DrawUtil.circlePointCache[j]), pos + quaternion * (radius * DrawUtil.circlePointCache[j + 1]), color, time);
		}
		global::UnityEngine.Debug.DrawLine(pos + quaternion * (radius * DrawUtil.circlePointCache[num - 1]), pos + quaternion * (radius * DrawUtil.circlePointCache[0]), color, time);
	}

	[Conditional("ENABLE_DEBUG_OUTPUT")]
	public static void Sphere(Vector3 pos, float radius)
	{
		DrawUtil.Sphere(pos, radius, Color.white, 0f);
	}

	[Conditional("ENABLE_DEBUG_OUTPUT")]
	public static void Sphere(Vector3 pos, float radius, Color color, float time = 0f)
	{
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[0] * radius, pos + DrawUtil.sphere_verts[1] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[1] * radius, pos + DrawUtil.sphere_verts[2] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[2] * radius, pos + DrawUtil.sphere_verts[3] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[3] * radius, pos + DrawUtil.sphere_verts[13] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[0] * radius, pos + DrawUtil.sphere_verts[4] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[4] * radius, pos + DrawUtil.sphere_verts[5] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[5] * radius, pos + DrawUtil.sphere_verts[6] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[6] * radius, pos + DrawUtil.sphere_verts[13] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[0] * radius, pos + DrawUtil.sphere_verts[7] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[7] * radius, pos + DrawUtil.sphere_verts[8] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[8] * radius, pos + DrawUtil.sphere_verts[9] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[9] * radius, pos + DrawUtil.sphere_verts[13] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[0] * radius, pos + DrawUtil.sphere_verts[10] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[10] * radius, pos + DrawUtil.sphere_verts[11] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[11] * radius, pos + DrawUtil.sphere_verts[12] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[12] * radius, pos + DrawUtil.sphere_verts[13] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[2] * radius, pos + DrawUtil.sphere_verts[14] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[8] * radius, pos + DrawUtil.sphere_verts[14] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[8] * radius, pos + DrawUtil.sphere_verts[15] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[5] * radius, pos + DrawUtil.sphere_verts[15] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[5] * radius, pos + DrawUtil.sphere_verts[16] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[11] * radius, pos + DrawUtil.sphere_verts[16] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[11] * radius, pos + DrawUtil.sphere_verts[17] * radius, color, time);
		global::UnityEngine.Debug.DrawLine(pos + DrawUtil.sphere_verts[2] * radius, pos + DrawUtil.sphere_verts[17] * radius, color, time);
	}

	private static Vector3[] sphere_verts = new Vector3[]
	{
		new Vector3(-1f, 0f, 0f),
		new Vector3(-0.7071f, -0.7071f, 0f),
		new Vector3(0f, -1f, 0f),
		new Vector3(0.7071f, -0.7071f, 0f),
		new Vector3(-0.7071f, 0.7071f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0.7071f, 0.7071f, 0f),
		new Vector3(-0.7071f, 0f, -0.7071f),
		new Vector3(0f, 0f, -1f),
		new Vector3(0.7071f, 0f, -0.7071f),
		new Vector3(-0.7071f, 0f, 0.7071f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0.7071f, 0f, 0.7071f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, -0.7071f, -0.7071f),
		new Vector3(0f, 0.7071f, -0.7071f),
		new Vector3(0f, 0.7071f, 0.7071f),
		new Vector3(0f, -0.7071f, 0.7071f)
	};

	private static Vector3[] circlePointCache;
}
