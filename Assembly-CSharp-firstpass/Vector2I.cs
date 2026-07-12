using System;
using System.Diagnostics;
using KSerialization;
using UnityEngine;
using YamlDotNet.Serialization;

[DebuggerDisplay("{x}, {y}")]
[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public struct Vector2I : IComparable<Vector2I>, IEquatable<Vector2I>
{
	public int X
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

	public int Y
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

	[YamlIgnore]
	public int sqrMagnitude
	{
		get
		{
			return Mathf.FloorToInt(Mathf.Pow((float)this.x, 2f) + Mathf.Pow((float)this.y, 2f));
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
	public Vector2I normalized
	{
		get
		{
			return this / this.magnitude;
		}
	}

	public Vector2I(int a, int b)
	{
		this.x = a;
		this.y = b;
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

	public static Vector2I Min(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x < w.x) ? v.x : w.x, (v.y < w.y) ? v.y : w.y);
	}

	public static Vector2I Max(Vector2I v, Vector2I w)
	{
		return new Vector2I((v.x > w.x) ? v.x : w.x, (v.y > w.y) ? v.y : w.y);
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

	public int magnitudeSqr
	{
		get
		{
			return this.x * this.x + this.y * this.y;
		}
	}

	public static implicit operator Vector2(Vector2I v)
	{
		return new Vector2((float)v.x, (float)v.y);
	}

	public override bool Equals(object obj)
	{
		bool flag;
		try
		{
			Vector2I vector2I = (Vector2I)obj;
			flag = vector2I.x == this.x && vector2I.y == this.y;
		}
		catch
		{
			flag = false;
		}
		return flag;
	}

	public bool Equals(Vector2I v)
	{
		return v.x == this.x && v.y == this.y;
	}

	public override int GetHashCode()
	{
		return this.x ^ this.y;
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

	public override string ToString()
	{
		return string.Format("{0}, {1}", this.x, this.y);
	}

	public int CompareTo(Vector2I other)
	{
		int num = this.y - other.y;
		if (other.y == 0)
		{
			return this.x - other.x;
		}
		return num;
	}

	public static readonly Vector2I zero = new Vector2I(0, 0);

	public static readonly Vector2I one = new Vector2I(1, 1);

	public static readonly Vector2I minusone = new Vector2I(-1, -1);

	[Serialize]
	public int x;

	[Serialize]
	public int y;
}
