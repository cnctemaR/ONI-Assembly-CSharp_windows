using System;
using UnityEngine;

[Serializable]
public struct CellOffset : IEquatable<CellOffset>
{
	public static CellOffset none
	{
		get
		{
			return new CellOffset(0, 0);
		}
	}

	public CellOffset(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public CellOffset(Vector2 offset)
	{
		this.x = Mathf.RoundToInt(offset.x);
		this.y = Mathf.RoundToInt(offset.y);
	}

	public Vector2I ToVector2I()
	{
		return new Vector2I(this.x, this.y);
	}

	public Vector3 ToVector3()
	{
		return new Vector3((float)this.x, (float)this.y, 0f);
	}

	public CellOffset Offset(CellOffset offset)
	{
		return new CellOffset(this.x + offset.x, this.y + offset.y);
	}

	public int GetOffsetDistance()
	{
		return Math.Abs(this.x) + Math.Abs(this.y);
	}

	public static CellOffset operator +(CellOffset a, CellOffset b)
	{
		return new CellOffset(a.x + b.x, a.y + b.y);
	}

	public static CellOffset operator -(CellOffset a, CellOffset b)
	{
		return new CellOffset(a.x - b.x, a.y - b.y);
	}

	public static CellOffset operator *(CellOffset offset, int value)
	{
		return new CellOffset(offset.x * value, offset.y * value);
	}

	public static CellOffset operator *(int value, CellOffset offset)
	{
		return new CellOffset(offset.x * value, offset.y * value);
	}

	public override bool Equals(object obj)
	{
		CellOffset cellOffset = (CellOffset)obj;
		return this.x == cellOffset.x && this.y == cellOffset.y;
	}

	public bool Equals(CellOffset offset)
	{
		return this.x == offset.x && this.y == offset.y;
	}

	public override int GetHashCode()
	{
		return this.x + this.y * 8192;
	}

	public static bool operator ==(CellOffset a, CellOffset b)
	{
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(CellOffset a, CellOffset b)
	{
		return a.x != b.x || a.y != b.y;
	}

	public override string ToString()
	{
		return string.Concat(new object[] { "(", this.x, ",", this.y, ")" });
	}

	public int x;

	public int y;
}
