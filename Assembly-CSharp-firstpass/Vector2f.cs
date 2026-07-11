using System;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[DebuggerDisplay("{x}, {y}")]
public struct Vector2f
{
	public float X
	{
		get
		{
			return this.x;
		}
		set
		{
			this.x = value;
		}
	}

	public float Y
	{
		get
		{
			return this.y;
		}
		set
		{
			this.y = value;
		}
	}

	public Vector2f(int a, int b)
	{
		this.x = (float)a;
		this.y = (float)b;
	}

	public Vector2f(float a, float b)
	{
		this.x = a;
		this.y = b;
	}

	public Vector2f(Vector2 src)
	{
		this.x = src.x;
		this.y = src.y;
	}

	public static bool operator ==(Vector2f u, Vector2f v)
	{
		return u.x == v.x && u.y == v.y;
	}

	public static bool operator !=(Vector2f u, Vector2f v)
	{
		return u.x != v.x || u.y != v.y;
	}

	public static implicit operator Vector2(Vector2f v)
	{
		return new Vector2(v.x, v.y);
	}

	public static implicit operator Vector2f(Vector2 v)
	{
		return new Vector2f(v.x, v.y);
	}

	public bool Equals(Vector2 v)
	{
		return v.x == this.x && v.y == this.y;
	}

	public override bool Equals(object obj)
	{
		bool flag;
		try
		{
			Vector2f vector2f = (Vector2f)obj;
			flag = vector2f.x == this.x && vector2f.y == this.y;
		}
		catch
		{
			flag = false;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return this.x.GetHashCode() ^ this.y.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}, {1}", this.x, this.y);
	}

	[Serialize]
	public float x;

	[Serialize]
	public float y;
}
