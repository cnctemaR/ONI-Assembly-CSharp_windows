using System;
using UnityEngine;

[Serializable]
public struct AABB3
{
	public AABB3(Vector3 pt)
	{
		this.min = pt;
		this.max = pt;
	}

	public AABB3(Vector3 min, Vector3 max)
	{
		this.min = min;
		this.max = max;
	}

	public bool IsValid()
	{
		return this.min.Min(this.max) == this.min;
	}

	public Vector3 Center
	{
		get
		{
			return (this.min + this.max) * 0.5f;
		}
	}

	public Vector3 Range
	{
		get
		{
			return this.max - this.min;
		}
	}

	public void Expand(float amount)
	{
		Vector3 vector = new Vector3(amount * 0.5f, amount * 0.5f, amount * 0.5f);
		this.min -= vector;
		this.max += vector;
	}

	public void ExpandToFit(Vector3 pt)
	{
		this.min = this.min.Min(pt);
		this.max = this.max.Max(pt);
	}

	public void ExpandToFit(AABB3 aabb)
	{
		this.min = this.min.Min(aabb.min);
		this.max = this.max.Max(aabb.max);
	}

	public bool Contains(Vector3 pt)
	{
		return this.min.LessEqual(pt) && pt.Less(this.max);
	}

	public bool Contains(AABB3 aabb)
	{
		return this.Contains(aabb.min) && this.Contains(aabb.max);
	}

	public bool Intersects(AABB3 aabb)
	{
		return this.min.LessEqual(aabb.max) && aabb.min.Less(this.max);
	}

	public override bool Equals(object obj)
	{
		bool flag;
		if (obj == null)
		{
			flag = false;
		}
		else
		{
			AABB3 aabb = (AABB3)obj;
			flag = this.min == aabb.min && this.max == aabb.max;
		}
		return flag;
	}

	public override int GetHashCode()
	{
		return this.min.GetHashCode() ^ this.max.GetHashCode();
	}

	public unsafe void Transform(Matrix4x4 t)
	{
		Vector3* ptr = stackalloc Vector3[checked(8 * sizeof(Vector3))];
		*ptr = this.min;
		ptr[1] = new Vector3(this.min.x, this.min.y, this.max.z);
		ptr[sizeof(Vector3) * 2 / sizeof(Vector3)] = new Vector3(this.min.x, this.max.y, this.min.z);
		ptr[sizeof(Vector3) * 3 / sizeof(Vector3)] = new Vector3(this.max.x, this.min.y, this.min.z);
		ptr[sizeof(Vector3) * 4 / sizeof(Vector3)] = new Vector3(this.min.x, this.max.y, this.max.z);
		ptr[sizeof(Vector3) * 5 / sizeof(Vector3)] = new Vector3(this.max.x, this.min.y, this.max.z);
		ptr[sizeof(Vector3) * 6 / sizeof(Vector3)] = new Vector3(this.max.x, this.max.y, this.min.z);
		ptr[sizeof(Vector3) * 7 / sizeof(Vector3)] = this.max;
		this.min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		this.max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		for (int i = 0; i < 8; i++)
		{
			this.ExpandToFit(t * ptr[i]);
		}
	}

	public float Width
	{
		get
		{
			return this.max.x - this.min.x;
		}
	}

	public float Height
	{
		get
		{
			return this.max.y - this.min.y;
		}
	}

	public float Depth
	{
		get
		{
			return this.max.z - this.min.z;
		}
	}

	public Vector3 min;

	public Vector3 max;
}
