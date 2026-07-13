using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	[Serializable]
	public struct float3x2 : IEquatable<float3x2>, IFormattable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(float3 c0, float3 c1)
		{
			this.c0 = c0;
			this.c1 = c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(float m00, float m01, float m10, float m11, float m20, float m21)
		{
			this.c0 = new float3(m00, m10, m20);
			this.c1 = new float3(m01, m11, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(float v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(bool v)
		{
			this.c0 = math.select(new float3(0f), new float3(1f), v);
			this.c1 = math.select(new float3(0f), new float3(1f), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(bool3x2 v)
		{
			this.c0 = math.select(new float3(0f), new float3(1f), v.c0);
			this.c1 = math.select(new float3(0f), new float3(1f), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(int v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(int3x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(uint v)
		{
			this.c0 = v;
			this.c1 = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(uint3x2 v)
		{
			this.c0 = v.c0;
			this.c1 = v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(double v)
		{
			this.c0 = (float3)v;
			this.c1 = (float3)v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float3x2(double3x2 v)
		{
			this.c0 = (float3)v.c0;
			this.c1 = (float3)v.c1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x2(float v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x2(bool v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x2(bool3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x2(int v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x2(int3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x2(uint v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float3x2(uint3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x2(double v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator float3x2(double3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator *(float3x2 lhs, float3x2 rhs)
		{
			return new float3x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator *(float3x2 lhs, float rhs)
		{
			return new float3x2(lhs.c0 * rhs, lhs.c1 * rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator *(float lhs, float3x2 rhs)
		{
			return new float3x2(lhs * rhs.c0, lhs * rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator +(float3x2 lhs, float3x2 rhs)
		{
			return new float3x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator +(float3x2 lhs, float rhs)
		{
			return new float3x2(lhs.c0 + rhs, lhs.c1 + rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator +(float lhs, float3x2 rhs)
		{
			return new float3x2(lhs + rhs.c0, lhs + rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator -(float3x2 lhs, float3x2 rhs)
		{
			return new float3x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator -(float3x2 lhs, float rhs)
		{
			return new float3x2(lhs.c0 - rhs, lhs.c1 - rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator -(float lhs, float3x2 rhs)
		{
			return new float3x2(lhs - rhs.c0, lhs - rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator /(float3x2 lhs, float3x2 rhs)
		{
			return new float3x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator /(float3x2 lhs, float rhs)
		{
			return new float3x2(lhs.c0 / rhs, lhs.c1 / rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator /(float lhs, float3x2 rhs)
		{
			return new float3x2(lhs / rhs.c0, lhs / rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator %(float3x2 lhs, float3x2 rhs)
		{
			return new float3x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator %(float3x2 lhs, float rhs)
		{
			return new float3x2(lhs.c0 % rhs, lhs.c1 % rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator %(float lhs, float3x2 rhs)
		{
			return new float3x2(lhs % rhs.c0, lhs % rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator ++(float3x2 val)
		{
			float3 @float = float3.op_Increment(val.c0);
			val.c0 = @float;
			float3 float2 = @float;
			@float = float3.op_Increment(val.c1);
			val.c1 = @float;
			return new float3x2(float2, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator --(float3x2 val)
		{
			float3 @float = float3.op_Decrement(val.c0);
			val.c0 = @float;
			float3 float2 = @float;
			@float = float3.op_Decrement(val.c1);
			val.c1 = @float;
			return new float3x2(float2, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(float3x2 lhs, float3x2 rhs)
		{
			return new bool3x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(float3x2 lhs, float rhs)
		{
			return new bool3x2(lhs.c0 < rhs, lhs.c1 < rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <(float lhs, float3x2 rhs)
		{
			return new bool3x2(lhs < rhs.c0, lhs < rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(float3x2 lhs, float3x2 rhs)
		{
			return new bool3x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(float3x2 lhs, float rhs)
		{
			return new bool3x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator <=(float lhs, float3x2 rhs)
		{
			return new bool3x2(lhs <= rhs.c0, lhs <= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(float3x2 lhs, float3x2 rhs)
		{
			return new bool3x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(float3x2 lhs, float rhs)
		{
			return new bool3x2(lhs.c0 > rhs, lhs.c1 > rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >(float lhs, float3x2 rhs)
		{
			return new bool3x2(lhs > rhs.c0, lhs > rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(float3x2 lhs, float3x2 rhs)
		{
			return new bool3x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(float3x2 lhs, float rhs)
		{
			return new bool3x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator >=(float lhs, float3x2 rhs)
		{
			return new bool3x2(lhs >= rhs.c0, lhs >= rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator -(float3x2 val)
		{
			return new float3x2(-val.c0, -val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 operator +(float3x2 val)
		{
			return new float3x2(+val.c0, +val.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(float3x2 lhs, float3x2 rhs)
		{
			return new bool3x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(float3x2 lhs, float rhs)
		{
			return new bool3x2(lhs.c0 == rhs, lhs.c1 == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator ==(float lhs, float3x2 rhs)
		{
			return new bool3x2(lhs == rhs.c0, lhs == rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(float3x2 lhs, float3x2 rhs)
		{
			return new bool3x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(float3x2 lhs, float rhs)
		{
			return new bool3x2(lhs.c0 != rhs, lhs.c1 != rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 operator !=(float lhs, float3x2 rhs)
		{
			return new bool3x2(lhs != rhs.c0, lhs != rhs.c1);
		}

		public unsafe ref float3 this[int index]
		{
			get
			{
				fixed (float3x2* ptr = &this)
				{
					return ref *(float3*)(ptr + (IntPtr)index * (IntPtr)sizeof(float3) / (IntPtr)sizeof(float3x2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(float3x2 rhs)
		{
			return this.c0.Equals(rhs.c0) && this.c1.Equals(rhs.c1);
		}

		public override bool Equals(object o)
		{
			if (o is float3x2)
			{
				float3x2 float3x = (float3x2)o;
				return this.Equals(float3x);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return (int)math.hash(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return string.Format("float3x2({0}f, {1}f,  {2}f, {3}f,  {4}f, {5}f)", new object[]
			{
				this.c0.x,
				this.c1.x,
				this.c0.y,
				this.c1.y,
				this.c0.z,
				this.c1.z
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format("float3x2({0}f, {1}f,  {2}f, {3}f,  {4}f, {5}f)", new object[]
			{
				this.c0.x.ToString(format, formatProvider),
				this.c1.x.ToString(format, formatProvider),
				this.c0.y.ToString(format, formatProvider),
				this.c1.y.ToString(format, formatProvider),
				this.c0.z.ToString(format, formatProvider),
				this.c1.z.ToString(format, formatProvider)
			});
		}

		public float3 c0;

		public float3 c1;

		public static readonly float3x2 zero;
	}
}
