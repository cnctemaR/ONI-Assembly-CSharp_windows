using System;
using System.Diagnostics;
using UnityEngine;
using YamlDotNet.Serialization;

[DebuggerDisplay("{x}, {y}, {z}")]
public struct Vector3I
{
	[YamlIgnore]
	public int sqrMagnitude
	{
		get
		{
			return Mathf.FloorToInt(Mathf.Pow((float)this.x, 2f) + Mathf.Pow((float)this.y, 2f) + Mathf.Pow((float)this.z, 2f));
		}
	}

	[YamlIgnore]
	public int magnitude
	{
		get
		{
			return Mathf.FloorToInt(Mathf.Sqrt((float)this.sqrMagnitude));
		}
	}

	[YamlIgnore]
	public Vector3I normalized
	{
		get
		{
			return this / this.magnitude;
		}
	}

	public Vector3I(int a, int b, int c)
	{
		this.x = a;
		this.y = b;
		this.z = c;
	}

	public static Vector3I operator +(Vector3I u, Vector3I v)
	{
		return new Vector3I(u.x + v.x, u.y + v.y, u.z + v.z);
	}

	public static Vector3I operator -(Vector3I u, Vector3I v)
	{
		return new Vector3I(u.x - v.x, u.y - v.y, u.z - v.z);
	}

	public static Vector3I operator *(Vector3I u, Vector3I v)
	{
		return new Vector3I(u.x * v.x, u.y * v.y, u.z * v.z);
	}

	public static Vector3I operator /(Vector3I u, Vector3I v)
	{
		return new Vector3I(u.x / v.x, u.y / v.y, u.z / v.z);
	}

	public static Vector3I operator *(Vector3I v, int s)
	{
		return new Vector3I(v.x * s, v.y * s, v.z * s);
	}

	public static Vector3I operator /(Vector3I v, int s)
	{
		return new Vector3I(v.x / s, v.y / s, v.z / s);
	}

	public static Vector3I operator +(Vector3I u, int scalar)
	{
		return new Vector3I(u.x + scalar, u.y + scalar, u.z + scalar);
	}

	public static Vector3I operator -(Vector3I u, int scalar)
	{
		return new Vector3I(u.x - scalar, u.y - scalar, u.z - scalar);
	}

	public static bool operator ==(Vector3I v1, Vector3I v2)
	{
		return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
	}

	public static bool operator !=(Vector3I v1, Vector3I v2)
	{
		return !(v1 == v2);
	}

	public override bool Equals(object o)
	{
		return base.Equals(o);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}, {1}, {2}", this.x, this.y, this.z);
	}

	public int x;

	public int y;

	public int z;
}
