using System;
using UnityEngine;

public struct Matrix2x3
{
	public Matrix2x3(float e00, float e01, float e02, float e10, float e11, float e12)
	{
		this.m00 = e00;
		this.m01 = e01;
		this.m02 = e02;
		this.m10 = e10;
		this.m11 = e11;
		this.m12 = e12;
	}

	public override bool Equals(object obj)
	{
		Matrix2x3 matrix2x = (Matrix2x3)obj;
		return this == matrix2x;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public Vector3 MultiplyPoint(Vector3 v)
	{
		return new Vector3(v.x * this.m00 + v.y * this.m01 + this.m02, v.x * this.m10 + v.y * this.m11 + this.m12, v.z);
	}

	public static Vector3 operator *(Matrix2x3 m, Vector3 v)
	{
		return new Vector3(v.x * m.m00 + v.y * m.m01 + m.m02, v.x * m.m10 + v.y * m.m11 + m.m12, v.z);
	}

	public static Matrix2x3 operator *(Matrix2x3 m, Matrix2x3 n)
	{
		return new Matrix2x3(m.m00 * n.m00 + m.m01 * n.m10, m.m00 * n.m01 + m.m01 * n.m11, m.m00 * n.m02 + m.m01 * n.m12 + m.m02 * 1f, m.m10 * n.m00 + m.m11 * n.m10, m.m10 * n.m01 + m.m11 * n.m11, m.m10 * n.m02 + m.m11 * n.m12 + m.m12 * 1f);
	}

	public static bool operator ==(Matrix2x3 m, Matrix2x3 n)
	{
		return m.m00 == n.m00 && m.m01 == n.m01 && m.m02 == n.m02 && m.m10 == n.m10 && m.m11 == n.m11 && m.m11 == n.m12;
	}

	public static bool operator !=(Matrix2x3 m, Matrix2x3 n)
	{
		return !(m == n);
	}

	public static implicit operator Matrix4x4(Matrix2x3 m)
	{
		Matrix4x4 matrix4x = Matrix4x4.identity;
		matrix4x.m00 = m.m00;
		matrix4x.m01 = m.m01;
		matrix4x.m03 = m.m02;
		matrix4x.m10 = m.m10;
		matrix4x.m11 = m.m11;
		matrix4x.m13 = m.m12;
		return matrix4x;
	}

	public float m00;

	public float m01;

	public float m02;

	public float m10;

	public float m11;

	public float m12;

	public static Matrix2x3 identity = new Matrix2x3(1f, 0f, 0f, 0f, 1f, 0f);
}
