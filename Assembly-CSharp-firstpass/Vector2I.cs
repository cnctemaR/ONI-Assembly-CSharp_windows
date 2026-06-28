using System;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{x}, {y}")]
[Serializable]
public struct Vector2I
{
	public Vector2I(int a, int b)
	{
		this.x = a;
		this.y = b;
	}

	public static Vector2I Min(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x >= w.x) ? w.x : v.x, (v.y >= w.y) ? w.y : v.y);
	}

	public static Vector2I Max(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x <= w.x) ? w.x : v.x, (v.y <= w.y) ? w.y : v.y);
	}

	public int magnitudeSqr
	{
		get
		{
			return this.x * this.x + this.y * this.y;
		}
	}

	public override bool Equals(object obj)
	{
		Vector2I vector2I = (Vector2I)obj;
		return vector2I.x == this.x && vector2I.y == this.y;
	}

	public override int GetHashCode()
	{
		return this.x ^ this.y;
	}

	public override string ToString()
	{
		return string.Format("{0}, {1}", this.x, this.y);
	}

	public static Vector2I operator +(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x + v.x, u.y + v.y);
	}

	public static Vector2I operator -(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x - v.x, u.y - v.y);
	}

	public static Vector2I operator *(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x * v.x, u.y * v.y);
	}

	public static Vector2I operator /(Vector2I u, Vector2I v)
	{
		return new Vector2I(u.x / v.x, u.y / v.y);
	}

	public static Vector2I operator *(Vector2I v, int s)
	{
		return new Vector2I(v.x * s, v.y * s);
	}

	public static Vector2I operator /(Vector2I v, int s)
	{
		return new Vector2I(v.x / s, v.y / s);
	}

	public static Vector2I operator +(Vector2I u, int scalar)
	{
		return new Vector2I(u.x + scalar, u.y + scalar);
	}

	public static Vector2I operator -(Vector2I u, int scalar)
	{
		return new Vector2I(u.x - scalar, u.y - scalar);
	}

	public static bool operator ==(Vector2I u, Vector2I v)
	{
		return u.x == v.x && u.y == v.y;
	}

	public static bool operator !=(Vector2I u, Vector2I v)
	{
		return u.x != v.x || u.y != v.y;
	}

	public static bool operator <(Vector2I u, Vector2I v)
	{
		return u.x < v.x && u.y < v.y;
	}

	public static bool operator >(Vector2I u, Vector2I v)
	{
		return u.x > v.x && u.y > v.y;
	}

	public static bool operator <=(Vector2I u, Vector2I v)
	{
		return u.x <= v.x && u.y <= v.y;
	}

	public static bool operator >=(Vector2I u, Vector2I v)
	{
		return u.x >= v.x && u.y >= v.y;
	}

	public static implicit operator Vector2(Vector2I v)
	{
		return new Vector2((float)v.x, (float)v.y);
	}

	public static bool operator <=(Vector2I u, Vector2 v)
	{
		return (float)u.x <= v.x && (float)u.y <= v.y;
	}

	public static bool operator >=(Vector2I u, Vector2 v)
	{
		return (float)u.x >= v.x && (float)u.y >= v.y;
	}

	public static bool operator <=(Vector2 u, Vector2I v)
	{
		return u.x <= (float)v.x && u.y <= (float)v.y;
	}

	public static bool operator >=(Vector2 u, Vector2I v)
	{
		return u.x >= (float)v.x && u.y >= (float)v.y;
	}

	public static readonly Vector2I zero = new Vector2I(0, 0);

	public static readonly Vector2I one = new Vector2I(1, 1);

	public static readonly Vector2I minusone = new Vector2I(-1, -1);

	[Serialize]
	public int x;

	[Serialize]
	public int y;
}
