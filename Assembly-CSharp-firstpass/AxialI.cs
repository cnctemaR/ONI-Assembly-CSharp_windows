using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[DebuggerDisplay("{r}, {q}")]
[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public struct AxialI : IEquatable<AxialI>
{
	public int R
	{
		get
		{
			return this.r;
		}
		set
		{
			this.r = value;
		}
	}

	public int Q
	{
		get
		{
			return this.q;
		}
		set
		{
			this.q = value;
		}
	}

	public AxialI(int a, int b)
	{
		this.r = a;
		this.q = b;
	}

	public Vector3I ToCube()
	{
		int num = this.q;
		int num2 = this.r;
		int num3 = -num - num2;
		return new Vector3I(num, num3, num2);
	}

	public Vector3 ToWorld()
	{
		return AxialUtil.AxialToWorld((float)this.r, (float)this.q);
	}

	public Vector2 ToWorld2D()
	{
		Vector3 vector = this.ToWorld();
		return new Vector2(vector.x, vector.y);
	}

	public static AxialI operator +(AxialI u, AxialI v)
	{
		return new AxialI(u.r + v.r, u.q + v.q);
	}

	public static AxialI operator -(AxialI u, AxialI v)
	{
		return new AxialI(u.r - v.r, u.q - v.q);
	}

	public static AxialI operator +(AxialI u, int scalar)
	{
		return new AxialI(u.r + scalar, u.q + scalar);
	}

	public static AxialI operator -(AxialI u, int scalar)
	{
		return new AxialI(u.r - scalar, u.q - scalar);
	}

	public static AxialI operator *(AxialI v, int s)
	{
		return new AxialI(v.r * s, v.q * s);
	}

	public static AxialI operator /(AxialI v, int s)
	{
		return new AxialI(v.r / s, v.q / s);
	}

	public static bool operator ==(AxialI u, AxialI v)
	{
		return u.r == v.r && u.q == v.q;
	}

	public static bool operator !=(AxialI u, AxialI v)
	{
		return u.r != v.r || u.q != v.q;
	}

	public static bool operator <(AxialI u, AxialI v)
	{
		return u.r < v.r && u.q < v.q;
	}

	public static bool operator >(AxialI u, AxialI v)
	{
		return u.r > v.r && u.q > v.q;
	}

	public static bool operator <=(AxialI u, AxialI v)
	{
		return u.r <= v.r && u.q <= v.q;
	}

	public static bool operator >=(AxialI u, AxialI v)
	{
		return u.r >= v.r && u.q >= v.q;
	}

	public override bool Equals(object obj)
	{
		bool flag;
		try
		{
			AxialI axialI = (AxialI)obj;
			flag = axialI.r == this.r && axialI.q == this.q;
		}
		catch
		{
			flag = false;
		}
		return flag;
	}

	public bool Equals(AxialI v)
	{
		return v.r == this.r && v.q == this.q;
	}

	public override int GetHashCode()
	{
		return this.r ^ this.q;
	}

	public static readonly AxialI INVALID = new AxialI(int.MaxValue, int.MaxValue);

	public static readonly AxialI ZERO = new AxialI(0, 0);

	public static readonly AxialI NORTHWEST = new AxialI(0, -1);

	public static readonly AxialI NORTHEAST = new AxialI(1, -1);

	public static readonly AxialI EAST = new AxialI(1, 0);

	public static readonly AxialI SOUTHEAST = new AxialI(0, 1);

	public static readonly AxialI SOUTHWEST = new AxialI(-1, 1);

	public static readonly AxialI WEST = new AxialI(-1, 0);

	public static readonly List<AxialI> DIRECTIONS = new List<AxialI>
	{
		AxialI.NORTHWEST,
		AxialI.NORTHEAST,
		AxialI.EAST,
		AxialI.SOUTHEAST,
		AxialI.SOUTHWEST,
		AxialI.WEST
	};

	public static readonly List<AxialI> CLOCKWISE = new List<AxialI>
	{
		AxialI.EAST,
		AxialI.SOUTHEAST,
		AxialI.SOUTHWEST,
		AxialI.WEST,
		AxialI.NORTHWEST,
		AxialI.NORTHEAST
	};

	[Serialize]
	public int r;

	[Serialize]
	public int q;
}
