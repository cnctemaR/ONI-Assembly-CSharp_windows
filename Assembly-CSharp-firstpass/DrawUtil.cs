using System;
using UnityEngine;

public static class DrawUtil
{
	public static void MultiColourGnomon(Vector2 pos, float size, float time = 0f)
	{
		size *= 0.5f;
	}

	public static void Gnomon(Vector3 pos, float size)
	{
		size *= 0.5f;
	}

	public static void Gnomon(Vector3 pos, float size, Color color, float time = 0f)
	{
		size *= 0.5f;
	}

	public static void Arrow(Vector3 start, Vector3 end, float size, Color color, float time = 0f)
	{
		Vector3 vector = end - start;
		if (vector.sqrMagnitude < 0.001f)
		{
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(vector, Vector3.up);
	}

	public static void Circle(Vector3 pos, float radius)
	{
		DrawUtil.Circle(pos, radius, Color.white, null, 0f);
	}

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
		}
	}

	public static void Sphere(Vector3 pos, float radius)
	{
		DrawUtil.Sphere(pos, radius, Color.white, 0f);
	}

	public static void Box(Vector3 pos, Color color, float size = 1f, float time = 1f)
	{
		float num = size * 0.5f;
	}

	public static void Sphere(Vector3 pos, float radius, Color color, float time = 0f)
	{
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
