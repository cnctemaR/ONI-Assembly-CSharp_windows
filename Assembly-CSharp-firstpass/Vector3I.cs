using System;
using System.Diagnostics;

[DebuggerDisplay("{x}, {y}, {z}")]
public struct Vector3I
{
	public Vector3I(int a, int b, int c)
	{
		this.x = a;
		this.y = b;
		this.z = c;
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

	public static bool operator ==(Vector3I v1, Vector3I v2)
	{
		return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
	}

	public static bool operator !=(Vector3I v1, Vector3I v2)
	{
		return !(v1 == v2);
	}

	public int x;

	public int y;

	public int z;
}
